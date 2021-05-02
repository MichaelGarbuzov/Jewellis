using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Jewellis.App_Custom.Services.ClientTheme
{
    /// <summary>
    /// Represents a service (scoped) for getting the client's preferred theme.
    /// </summary>
    public class ClientThemeService
    {
        #region Private Members

        private const string THEME_COOKIE = AppKeys.Cookies.ClientTheme;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the options configured for the <see cref="ClientThemeService"/>.
        /// </summary>
        public ClientThemeOptions Options { get; private set; }

        #endregion

        /// <summary>
        /// Represents a service (scoped) for getting the client's preferred theme.
        /// </summary>
        /// <param name="options">The options to configure the <see cref="ClientThemeService"/>.</param>
        public ClientThemeService(IOptions<ClientThemeOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            if (string.IsNullOrEmpty(options.Value.DefaultTheme))
                throw new ArgumentNullException("{options.DefaultTheme} cannot be null or empty.");
            if (options.Value.SupportedThemes == null)
                throw new ArgumentNullException("{options.SupportedThemes} cannot be null.");
            if (options.Value.SupportedThemes.Length < 1)
                throw new ArgumentException("{options.SupportedThemes} must have at least 1 supported theme.");

            this.Options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Public API

        /// <summary>
        /// Gets the current client's theme, either from the database (if user is registered) or the cookie.
        /// </summary>
        /// <returns>Returns the <see cref="Theme"/> of the current client.</returns>
        public Theme GetCurrent()
        {
            // TODO: check if user is registered:


            // User is not registered - so gets the theme from the cookie:
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[THEME_COOKIE];

            foreach (Theme theme in this.Options.SupportedThemes)
            {
                if (theme.CookieValue.Equals(cookieValue, StringComparison.InvariantCultureIgnoreCase))
                    return theme;
            }

            // If supported theme was not found - then sets the default:
            Theme defaultTheme = this.Options.SupportedThemes.FirstOrDefault(t => t.ID.Equals(this.Options.DefaultTheme));
            _httpContextAccessor.HttpContext.Response.Cookies.Append(THEME_COOKIE, defaultTheme.CookieValue, new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTimeOffset.Now.AddYears(100)
            });
            return defaultTheme;
        }

        #endregion

    }
}
