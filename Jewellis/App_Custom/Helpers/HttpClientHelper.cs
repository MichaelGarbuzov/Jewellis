using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jewellis.App_Custom.Helpers
{
    /// <summary>
    /// Represents a helper for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientHelper
    {

        #region Public Static API

        /// <summary>
        /// Sends a GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to request.</param>
        /// <returns>Returns the text response of the request.</returns>
        public static async Task<string> GetAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), $"{nameof(url)} cannot be null or empty.");

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Sends a GET request to the specified URL, and parses the response (must be a JSON) to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse the response to.</typeparam>
        /// <param name="url">The URL to request.</param>
        /// <returns>Returns the specified object parsed from the response of the request.</returns>
        public static async Task<T> GetObjectAsync<T>(string url) where T : class
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), $"{nameof(url)} cannot be null or empty.");

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseText);
            }
        }

        #endregion

    }
}
