using Jewellis.Data;
using Jewellis.Models;
using Jewellis.WebServices.CurrencyConverterApi;
using Jewellis.WebServices.IpApi;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
        #region Constants

        private const string CURRENCY_COOKIE = AppKeys.Cookies.ClientCurrency;
        private const string CACHE_IDENTIFIER = AppKeys.Cache.ClientCurrency;

        /// <summary>
        /// Holds a constant for the base currency to make conversions with.
        /// </summary>
        public const string BASE_CURRENCY = "USD";

        #endregion

        #region Private Members

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly JewellisDbContext _dbContext;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the options configured for the <see cref="ClientCurrencyService"/>.
        /// </summary>
        public ClientCurrencyOptions Options { get; private set; }

        /// <summary>
        /// Gets the current client's currency in this scope.
        /// </summary>
        public Currency Currency { get; private set; }

        /// <summary>
        /// Gets the current client's currency conversion rate in this scope.
        /// </summary>
        /// <remarks>
        /// Returns the conversion rate from the base currency (<see cref="BASE_CURRENCY"/>) to the current currency of the client.
        /// </remarks>
        public double ConversionRate { get; private set; }

        #endregion

        /// <summary>
        /// Represents a service (scoped) for managing the client's preferred currency.
        /// </summary>
        /// <param name="options">The options to configure the <see cref="ClientCurrencyService"/>.</param>
        public ClientCurrencyService(IOptions<ClientCurrencyOptions> options, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IMemoryCache cache, JewellisDbContext dbContext)
        {
            if (string.IsNullOrEmpty(options.Value.DefaultCurrency))
                throw new ArgumentNullException("{options.DefaultCurrency} cannot be null or empty.");
            if (options.Value.SupportedCurrencies == null)
                throw new ArgumentNullException("{options.SupportedCurrencies} cannot be null.");
            if (options.Value.SupportedCurrencies.Length < 1)
                throw new ArgumentException("{options.SupportedCurrencies} must have at least 1 supported theme.");

            this.Options = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _cache = cache;
            _dbContext = dbContext;

            this.InitializeClientCurrency();
        }

        #region Public API

        /// <summary>
        /// Sets the current client's currency, either to the database (if user is registered) or the cookie.
        /// </summary>
        /// <param name="currencyCode">The code (id) of the supported currency to set.</param>
        public async Task SetAsync(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
                throw new ArgumentNullException(nameof(currencyCode), $"{nameof(currencyCode)} cannot be null.");

            // Ensures a supported currency:
            Currency currency = this.GetSupportedCurrencyOrNull(currencyCode);
            if (currency == null)
                currency = this.Options.SupportedCurrencies.FirstOrDefault(c => c.Code.Equals(this.Options.DefaultCurrency));

            // Sets the currency:
            // Checks if the user is registered, to assign the currency to the database:
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                await this.SetUserCurrencyToDatabase(currency);
            }

            // Assigns the currency to the cookie:
            this.SetUserCurrencyToCookie(currency);

            // Assigns the currency to the current scope:
            this.Currency = currency;
        }

        /// <summary>
        /// Gets the price value in the client's currency, after conversion from the base currency (<see cref="BASE_CURRENCY"/>) to client's current currency.
        /// </summary>
        /// <param name="price">The price in the base currency (<see cref="BASE_CURRENCY"/>).</param>
        /// <returns>Returns the price value in the client's currency, after conversion from the base currency (<see cref="BASE_CURRENCY"/>) to client's current currency.</returns>
        public double GetPrice(double price)
        {
            return Math.Round(price * this.ConversionRate, 2);
        }

        /// <summary>
        /// Gets the price value in the base currency (<see cref="BASE_CURRENCY"/>), after conversion from the client's current currency to the base currency (<see cref="BASE_CURRENCY"/>).
        /// </summary>
        /// <param name="price">The price in the client's current currency.</param>
        /// <returns>Returns the price value in the base currency (<see cref="BASE_CURRENCY"/>), after conversion from the client's current currency to the base currency (<see cref="BASE_CURRENCY"/>)</returns>
        public double GetBasePrice(double price)
        {
            return Math.Round(price / this.ConversionRate, 2);
        }

        /// <summary>
        /// Gets the price value in the client's currency, after conversion from the base currency (<see cref="BASE_CURRENCY"/>) to client's current currency, then displays it.
        /// </summary>
        /// <param name="price">The price in the base currency (<see cref="BASE_CURRENCY"/>).</param>
        /// <returns>Returns the display format of the price value in the client's currency, after conversion from the base currency (<see cref="BASE_CURRENCY"/>) to client's current currency.</returns>
        public string GetPriceAndDisplay(double price)
        {
            double convertedPrice = this.GetPrice(price);
            return string.Format("{0}{1:0.00}", this.Currency.Symbol, convertedPrice);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the client's currency, either from the database (if user is registered), the cookie, or a web service to detect currency by IP address.
        /// </summary>
        private void InitializeClientCurrency()
        {
            // The following (4) steps are looking for a supported currency in various methods:
            Currency userCurrency = null;

            // (1) - Checks if the user is authenticated - then gets the currency from the DB:
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                userCurrency = Task.Run(() => GetUserCurrencyByDatabase()).Result;

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
            this.SetUserCurrencyToCookie(userCurrency);
            // Assigns the currency found to the current scope:
            this.Currency = userCurrency;

            // Initializes the conversion rate:
            this.ConversionRate = this.GetConversionRate(this.Currency);
        }

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
                if (string.Equals(currency.Code, currencyCode, StringComparison.OrdinalIgnoreCase))
                    return currency;
            }
            return null;
        }

        /// <summary>
        /// Gets the currency of the user by the database (for an authenticated user).
        /// </summary>
        /// <returns>Returns the currency of the user by the database if found and supported, otherwise null.</returns>
        private async Task<Currency> GetUserCurrencyByDatabase()
        {
            int? userId = _httpContextAccessor.HttpContext.User.Identity.GetId();
            if (userId != null)
            {
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user != null)
                {
                    return this.GetSupportedCurrencyOrNull(user.Currency);
                }
            }
            return null;
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
        /// Sets the specified currency to the current authenticated user in the database.
        /// </summary>
        /// <param name="currency">The currency to set to the current user.</param>
        private async Task SetUserCurrencyToDatabase(Currency currency)
        {
            int? userId = _httpContextAccessor.HttpContext.User.Identity.GetId();
            if (userId != null)
            {
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user != null)
                {
                    user.Currency = currency.Code;
                    user.DateLastModified = DateTime.Now;

                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Sets the specified currency to the current user's cookie.
        /// </summary>
        /// <param name="currency">The currency to set to the current user's cookie.</param>
        private void SetUserCurrencyToCookie(Currency currency)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CURRENCY_COOKIE, currency.Code, new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTimeOffset.Now.AddYears(100)
            });
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

        /// <summary>
        /// Returns the conversion rate from the base currency (<see cref="BASE_CURRENCY"/>) to the specified currency.
        /// Using a web service to get the conversion rate.
        /// </summary>
        /// <returns>Returns the conversion rate from the base currency (<see cref="BASE_CURRENCY"/>) to the specified currency.</returns>
        private double GetConversionRate(Currency currency)
        {
            double conversionRate;

            // Checks if it's the base currency, no conversion is needed:
            if (string.Equals(currency.Code, BASE_CURRENCY, StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }
            // Otherwise, checks if the currencies already converted in the cache:
            else if (_cache.TryGetValue($"{CACHE_IDENTIFIER}_{BASE_CURRENCY}_{currency.Code}", out conversionRate))
            {
                return conversionRate;
            }
            // Otherwise, gets the conversion rate by web service:
            else
            {
                conversionRate = this.GetConversionRateByWebService(BASE_CURRENCY, currency.Code);

                // Enters the conversion rate to cache (in order to reduce calls):
                _cache.Set($"{CACHE_IDENTIFIER}_{BASE_CURRENCY}_{currency.Code}", conversionRate, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });
                return conversionRate;
            }
        }

        /// <summary>
        /// Converts from one currency to another, using the <see cref="CurrencyConverterApiService"/> web service.
        /// </summary>
        /// <param name="fromCurrency">The currency to convert from.</param>
        /// <param name="toCurrency">The currency to convert to.</param>
        /// <returns>Returns the conversion rate from one currency to another, using the <see cref="CurrencyConverterApiService"/> web service.</returns>
        private double GetConversionRateByWebService(string fromCurrency, string toCurrency)
        {
            string ccApiKey = _configuration.GetSection("UserSecrets").GetSection("WebServicesCredentials")["CurrencyConverterApi"];
            CurrencyConverterApiService ccApiService = new CurrencyConverterApiService(ccApiKey);
            double conversionRate = Task.Run(() => ccApiService.ConvertAsync(fromCurrency, toCurrency)).Result;
            return conversionRate;
        }

        #endregion

    }
}
