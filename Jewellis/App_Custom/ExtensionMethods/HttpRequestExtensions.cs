using Microsoft.AspNetCore.Http;
using System;

namespace Jewellis
{
    public static class HttpRequestExtensions
    {

        /// <summary>
        /// Checks whether the current request is AJAX or not.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <returns>Returns true if the current request is AJAX, otherwise false.</returns>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
            {
                return !string.IsNullOrEmpty(request.Headers["X-Requested-With"]) &&
                    string.Equals(request.Headers["X-Requested-With"], "XmlHttpRequest", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

    }
}
