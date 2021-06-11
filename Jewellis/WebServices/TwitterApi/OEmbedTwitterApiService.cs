using Jewellis.App_Custom.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Jewellis.WebServices.TwitterApi
{
    /// <summary>
    /// Represents the interactions with the "oEmbed API - Twitter API" web service.
    /// For documentation see: https://developer.twitter.com/en/docs/twitter-for-websites/oembed-api
    /// </summary>
    public class OEmbedTwitterApiService
    {
        #region Constants

        /// <summary>
        /// Holds a constant of the base URL for interacting with the service.
        /// </summary>
        private const string BASE_URL = @"https://publish.twitter.com/oembed";

        /// <summary>
        /// Holds a constant for the property name: "html".
        /// </summary>
        private const string PARAM_HTML = "html";

        #endregion

        #region Public API

        /// <summary>
        /// Gets the embedded twitter timeline HTML of the specified twitter username.
        /// </summary>
        /// <param name="twitterUsername">The twitter username to get the embedded timeline.</param>
        /// <param name="limit">The number of maximum tweets to get.</param>
        /// <param name="darkTheme">Indicates if to use dark theme instead of light theme.</param>
        /// <returns>Returns the embedded twitter timeline HTML of the specified twitter username.</returns>
        public async Task<string> GetEmbeddedTimelineAsync(string twitterUsername, int limit = 2, bool darkTheme = false)
        {
            if (string.IsNullOrEmpty(twitterUsername))
                throw new ArgumentNullException(nameof(twitterUsername), $"{nameof(twitterUsername)} cannot be null or empty.");

            string url = $"{BASE_URL}?limit={limit}&url=https://twitter.com/{twitterUsername}";
            if (darkTheme)
            {
                url = $"{url}&theme=dark";
            }
            string response = await HttpClientHelper.GetAsync(url);
            return JObject.Parse(response).GetValue(PARAM_HTML).ToString();
        }

        #endregion

    }
}
