using Jewellis.App_Custom.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Jewellis.WebServices.IpApi
{
    /// <summary>
    /// Represents the interactions with the "Ip-Api" web service.
    /// For documentation see: https://ip-api.com/
    /// </summary>
    public class IpApiWebService
    {
        #region Constants

        /// <summary>
        /// Holds a constant of the base URL for interacting with the service.
        /// </summary>
        private const string BASE_URL = @"http://ip-api.com/json";

        /// <summary>
        /// Holds a constant for the property name: "currency".
        /// </summary>
        private const string PARAM_CURRENCY_CODE = "currency";

        #endregion

        #region Public API

        /// <summary>
        /// Gets the currency code associated with the locale of the specified IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to get its locale currency code.</param>
        /// <returns>Returns the currency code associated with the locale of the specified IP address.</returns>
        public async Task<string> GetCurrencyCodeAsync(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress), $"{nameof(ipAddress)} cannot be null or empty.");

            IPAddress ip;
            if (!IPAddress.TryParse(ipAddress, out ip) || ipAddress.Equals("::1"))
                return null;

            string url = $"{BASE_URL}/{ipAddress}?fields={PARAM_CURRENCY_CODE}";

            string response = await HttpClientHelper.GetAsync(url);
            return JObject.Parse(response).GetValue(PARAM_CURRENCY_CODE).ToString();
        }

        #endregion

    }
}
