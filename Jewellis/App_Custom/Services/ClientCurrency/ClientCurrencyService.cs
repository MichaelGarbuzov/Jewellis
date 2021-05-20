using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

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
        /// Gets the current client's currency, either from the database (if user is registered) or the cookie.
        /// </summary>
        /// <returns>Returns the <see cref="Currency"/> of the current client.</returns>
        public Currency GetCurrent()
        {
            // TODO: check if user is registered:


            // User is not registered - so gets the currency from the cookie:
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[CURRENCY_COOKIE];

            foreach (Currency currency in this.Options.SupportedCurrencies)
            {
                if (currency.Code.Equals(cookieValue, StringComparison.InvariantCultureIgnoreCase))
                    return currency;
            }

            // If supported currency was not found - then sets the default:
            Currency defaultCurrency = this.Options.SupportedCurrencies.FirstOrDefault(c => c.Code.Equals(this.Options.DefaultCurrency));
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CURRENCY_COOKIE, defaultCurrency.Code, new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTimeOffset.Now.AddYears(100)
            });
            return defaultCurrency;
        }

        #endregion

    }
}
