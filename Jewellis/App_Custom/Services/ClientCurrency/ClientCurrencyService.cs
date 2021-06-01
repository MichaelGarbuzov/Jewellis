using Jewellis.WebServices.IpApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Jewellis.App_Custom.Services.ClientCurrency
{
    /// <summary>
    /// Represents a service (scoped) for managing the client's preferred currency.
    /// </summary>
    public class ClientCurrencyService
    {
        #region Private Members

        private const string CURRENCY_COOKIE = AppKeys.Cookies.ClientCurrency;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the options configured for the <see cref="ClientCurrencyService"/>.
        /// </summary>
        public ClientCurrencyOptions Options { get; private set; }

        #endregion

        /// <summary>
        /// Represents a service (scoped) for managing the client's preferred currency.
        /// </summary>
        /// <param name="options">The options to configure the <see cref="ClientCurrencyService"/>.</param>
        public ClientCurrencyService(IOptions<ClientCurrencyOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            if (string.IsNullOrEmpty(options.Value.DefaultCurrency))
                throw new ArgumentNullException("{options.DefaultCurrency} cannot be null or empty.");
            if (options.Value.SupportedCurrencies == null)
                throw new ArgumentNullException("{options.SupportedCurrencies} cannot be null.");
            if (options.Value.SupportedCurrencies.Length < 1)
                throw new ArgumentException("{options.SupportedCurrencies} must have at least 1 supported theme.");

            this.Options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Public API

        /// <summary>
        /// Gets the current client's currency, either from the database (if user is registered), the cookie, or a web service to detect currency by IP address.
        /// </summary>
        /// <returns>Returns the <see cref="Currency"/> of the current client.</returns>
        public Currency GetCurrent()
        {
            // The following (4) steps are looking for a supported currency in various methods:
            Currency userCurrency = null;

            // TODO: check if user is registered:
            //// (1) - Checks if the user is authenticated - then gets the currency from the DB:
            // if (user.isAuth())
            // userCurrency = this.GetUserCurrencyFromDatabase()

            // (2) - User is not authenticated - so gets the currency from the cookie:
            if (userCurrency == null)
                userCurrency = this.GetUserCurrencyByCookie();

            // (3) - Cookie value does not exist - so detects it by IP address:
            if (userCurrency == null)
                userCurrency = this.DetectUserCurrencyByWebService();

            // (4) - If supported currency was not found on all methods - then sets the default:
            if (userCurrency == null)
                userCurrency = this.Options.SupportedCurrencies.FirstOrDefault(c => c.Code.Equals(this.Options.DefaultCurrency));

            // Finally, assigns the currency to the cookie:
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CURRENCY_COOKIE, userCurrency.Code, new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTimeOffset.Now.AddYears(100)
            });
            return userCurrency;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the supported currency for the specified currency code, or null if not supported.
        /// </summary>
        /// <param name="currencyCode">The currency code to get.</param>
        /// <returns>Returns the supported currency for the specified currency code, or null if not supported.</returns>
        private Currency GetSupportedCurrencyOrNull(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
                return null;

            foreach (Currency currency in this.Options.SupportedCurrencies)
            {
                if (currency.Code.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase))
                    return currency;
            }
            return null;
        }

        /// <summary>
        /// Gets the currency of the user by the database (for an authenticated user).
        /// </summary>
        /// <returns>Returns the currency of the user by the database if found and supported, otherwise null.</returns>
        private Currency GetUserCurrencyByDatabase()
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the currency of the user by the cookie.
        /// </summary>
        /// <returns>Returns the currency of the user by the cookie if found and supported, otherwise null.</returns>
        private Currency GetUserCurrencyByCookie()
        {
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[CURRENCY_COOKIE];
            return this.GetSupportedCurrencyOrNull(cookieValue);
        }

        /// <summary>
        /// Detects the currency of the user by a web service that identifies the locale for a given IP address.
        /// </summary>
        /// <returns>Returns the currency of the user by the web service if found and supported, otherwise null.</returns>
        private Currency DetectUserCurrencyByWebService()
        {
            IPAddress userIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            if (userIpAddress == null)
                return null;

            try
            {
                IpApiWebService ipApiService = new IpApiWebService();
                string ipCurrency = Task.Run(() => ipApiService.GetCurrencyCodeAsync(userIpAddress.ToString())).Result;
                return this.GetSupportedCurrencyOrNull(ipCurrency);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

    }
}
