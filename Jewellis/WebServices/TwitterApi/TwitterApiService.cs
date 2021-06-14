using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jewellis.WebServices.TwitterApi
{
    /// <summary>
    /// Represents the interactions with the "Twitter API" web service.
    /// For documentation see: https://developer.twitter.com/en/docs/twitter-api
    /// </summary>
    public class TwitterApiService
    {
        #region Constants

        /// <summary>
        /// Holds a constant of the base URL for interacting with the service.
        /// </summary>
        private const string BASE_URL = @"https://api.twitter.com";

        /// <summary>
        /// Holds a constant of the base URL for interacting with the service.
        /// </summary>
        private const string API_VERSION = "1.1";

        #endregion

        #region Private Members

        private readonly HMACSHA1 _sigHasher;
        private readonly DateTime _epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly string _apiKey;
        private readonly string _apiSecretKey;
        private readonly string _oauthToken;
        private readonly string _oauthTokenSecret;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterApiService"/> class.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiSecretKey"></param>
        /// <param name="oauthToken"></param>
        /// <param name="oauthTokenSecret"></param>
        public TwitterApiService(string apiKey, string apiSecretKey, string oauthToken, string oauthTokenSecret)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey), $"{nameof(apiKey)} cannot be null or empty.");
            if (string.IsNullOrEmpty(apiSecretKey))
                throw new ArgumentNullException(nameof(apiSecretKey), $"{nameof(apiSecretKey)} cannot be null or empty.");
            if (string.IsNullOrEmpty(oauthToken))
                throw new ArgumentNullException(nameof(oauthToken), $"{nameof(oauthToken)} cannot be null or empty.");
            if (string.IsNullOrEmpty(oauthTokenSecret))
                throw new ArgumentNullException(nameof(oauthTokenSecret), $"{nameof(oauthTokenSecret)} cannot be null or empty.");

            _apiKey = apiKey;
            _apiSecretKey = apiSecretKey;
            _oauthToken = oauthToken;
            _oauthTokenSecret = oauthTokenSecret;
            _sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes($"{_apiSecretKey}&{_oauthTokenSecret}"));
        }

        #region Public API

        /// <summary>
        /// Posts a tweet to Twitter by the specified text.
        /// </summary>
        /// <param name="text">The text to tweet.</param>
        /// <returns></returns>
        public Task<string> PostTweetAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text), $"{nameof(text)} cannot be null or empty.");

            var data = new Dictionary<string, string> {
                { "status", text }
            };

            return this.SendRequest("statuses/update.json", data);
        }

        #endregion

        #region Private Methods

        private Task<string> SendRequest(string urlMethod, Dictionary<string, string> data)
        {
            string url = ($"{BASE_URL}/{API_VERSION}/{urlMethod}");

            // Timestamps are in seconds since 1/1/1970.
            int timestamp = (int)((DateTime.UtcNow - _epochUtc).TotalSeconds);

            // Adds all the OAuth headers we'll need to use when constructing the hash.
            data.Add("oauth_consumer_key", _apiKey);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            data.Add("oauth_timestamp", timestamp.ToString());
            data.Add("oauth_nonce", Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString())));
            data.Add("oauth_token", _oauthToken);
            data.Add("oauth_version", "1.0");

            // Generates the OAuth signature and add it to our payload.
            data.Add("oauth_signature", this.GenerateSignature(url, data));

            // Builds the OAuth HTTP Header from the data.
            string oAuthHeader = this.GenerateOAuthHeader(data);

            // Builds the form data (exclude OAuth stuff that's already in the header).
            var formData = new FormUrlEncodedContent(data.Where(kvp => !kvp.Key.StartsWith("oauth_")));

            return this.SendRequest(url, oAuthHeader, formData);
        }

        /// <summary>
        /// Generates an OAuth signature from OAuth header values.
        /// </summary>
        private string GenerateSignature(string url, Dictionary<string, string> data)
        {
            string sigString = string.Join(
                "&",
                data.Union(data)
                    .Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );
            string fullSigData = string.Format("{0}&{1}&{2}", "POST", Uri.EscapeDataString(url), Uri.EscapeDataString(sigString.ToString()));
            return Convert.ToBase64String(_sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigData.ToString())));
        }

        /// <summary>
        /// Generates the raw OAuth HTML header from the values (including signature).
        /// </summary>
        private string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return "OAuth " + string.Join(
                ", ",
                data.Where(kvp => kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );
        }

        /// <summary>
        /// Sends an HTTP Request and returns the response.
        /// </summary>
        private async Task<string> SendRequest(string fullUrl, string oAuthHeader, FormUrlEncodedContent formData)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", oAuthHeader);
                var response = await httpClient.PostAsync(fullUrl, formData);
                string responseText = await response.Content.ReadAsStringAsync();
                return responseText;
            }
        }

        #endregion

    }
}
