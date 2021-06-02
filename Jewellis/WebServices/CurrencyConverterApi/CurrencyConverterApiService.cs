using Jewellis.App_Custom.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Jewellis.WebServices.CurrencyConverterApi
{
    /// <summary>
    /// Represents the interactions with the "Currency Converter Api" web service.
    /// For documentation see: https://www.currencyconverterapi.com/
    /// </summary>
    public class CurrencyConverterApiService
    {
        #region Constants

        /// <summary>
        /// Holds a constant of the base URL for interacting with the service.
        /// </summary>
        private const string BASE_URL = @"https://free.currconv.com/api";

        /// <summary>
        /// Holds a constant of the base URL for interacting with the service.
        /// </summary>
        private const string API_VERSION = "v7";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the API key for the service.
        /// </summary>
        public string ApiKey { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyConverterApiService"/> class.
        /// </summary>
        /// <param name="apiKey">The api key to use with the service.</param>
        public CurrencyConverterApiService(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey), $"{nameof(apiKey)} cannot be null or empty.");

            this.ApiKey = apiKey;
        }

        #region Public API

        /// <summary>
        /// Gets the conversion rate from one currency to another.
        /// </summary>
        /// <param name="fromCurrency">The currency code to convert from.</param>
        /// <param name="toCurrency">The currency code to convert to.</param>
        /// <returns>Returns the conversion rate from one currency to another.</returns>
        public async Task<double> ConvertAsync(string fromCurrency, string toCurrency)
        {
            if (string.IsNullOrEmpty(fromCurrency))
                throw new ArgumentNullException(nameof(fromCurrency), $"{nameof(fromCurrency)} cannot be null or empty.");
            if (string.IsNullOrEmpty(toCurrency))
                throw new ArgumentNullException(nameof(toCurrency), $"{nameof(toCurrency)} cannot be null or empty.");

            string currencies = $"{fromCurrency}_{toCurrency}";
            string url = $"{BASE_URL}/{API_VERSION}/convert?apiKey={ApiKey}&compact=ultra&q={currencies}";

            string response = await HttpClientHelper.GetAsync(url);
            return JObject.Parse(response).GetValue(currencies).Value<double>();
        }

        #endregion

    }
}
