using Jewellis.App_Custom.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Jewellis.WebServices.CurrencyFreaksApi
{
    /// <summary>
    /// Represents the interactions with the "CurrencyFreaks API" web service.
    /// For documentation see: https://currencyfreaks.com/
    /// </summary>
    public class CurrencyFreaksApiService
    {
        #region Constants

        /// <summary>
        /// Holds a constant of the base URL for interacting with the service.
        /// </summary>
        private const string BASE_URL = @"https://api.currencyfreaks.com";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the API key for the service.
        /// </summary>
        public string ApiKey { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyFreaksApiService"/> class.
        /// </summary>
        /// <param name="apiKey">The api key to use with the service.</param>
        public CurrencyFreaksApiService(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey), $"{nameof(apiKey)} cannot be null or empty.");

            this.ApiKey = apiKey;
        }

        #region Public API

        /// <summary>
        /// Gets the conversion rate from USD to the specified currency.
        /// </summary>
        /// <param name="toCurrency">The currency code to convert to.</param>
        /// <returns>Returns the conversion rate from USD to the specified currency.</returns>
        public async Task<double> ConvertAsync(string toCurrency)
        {
            if (string.IsNullOrEmpty(toCurrency))
                throw new ArgumentNullException(nameof(toCurrency), $"{nameof(toCurrency)} cannot be null or empty.");

            string url = $"{BASE_URL}/latest?apikey={ApiKey}&symbols={toCurrency}";

            string response = await HttpClientHelper.GetAsync(url);
            return JObject.Parse(response)["rates"][toCurrency].Value<double>();
        }

        #endregion

    }
}
