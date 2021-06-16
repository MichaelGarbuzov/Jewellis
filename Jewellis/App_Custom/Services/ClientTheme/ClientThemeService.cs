using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.App_Custom.Services.ClientTheme
{
    /// <summary>
    /// Represents a service (scoped) for getting the client's preferred theme.
    /// </summary>
    public class ClientThemeService
    {
        #region Constants

        private const string THEME_COOKIE = AppKeys.Cookies.ClientTheme;

        #endregion

        #region Private Members

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JewellisDbContext _dbContext;
        private readonly UserIdentityService _userIdentity;
        private readonly UsersService _users;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the options configured for the <see cref="ClientThemeService"/>.
        /// </summary>
        public ClientThemeOptions Options { get; private set; }

        /// <summary>
        /// Gets the current client's theme in this scope.
        /// </summary>
        public Theme Theme { get; private set; }

        #endregion

        /// <summary>
        /// Represents a service (scoped) for getting the client's preferred theme.
        /// </summary>
        /// <param name="options">The options to configure the <see cref="ClientThemeService"/>.</param>
        public ClientThemeService(IOptions<ClientThemeOptions> options, IHttpContextAccessor httpContextAccessor, JewellisDbContext dbContext, UserIdentityService userIdentity, UsersService users)
        {
            if (string.IsNullOrEmpty(options.Value.DefaultTheme))
                throw new ArgumentNullException("{options.DefaultTheme} cannot be null or empty.");
            if (options.Value.SupportedThemes == null)
                throw new ArgumentNullException("{options.SupportedThemes} cannot be null.");
            if (options.Value.SupportedThemes.Length < 1)
                throw new ArgumentException("{options.SupportedThemes} must have at least 1 supported theme.");

            this.Options = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _userIdentity = userIdentity;
            _users = users;

            this.InitializeClientTheme();
        }

        #region Public API

        /// <summary>
        /// Sets the current client's theme, either to the database (if user is registered) or the cookie.
        /// </summary>
        /// <param name="themeId">The id of the supported theme to set.</param>
        public async Task SetAsync(string themeId)
        {
            if (string.IsNullOrEmpty(themeId))
                throw new ArgumentNullException(nameof(themeId), $"{nameof(themeId)} cannot be null.");

            // Ensures a supported theme:
            Theme theme = this.GetSupportedThemeOrNull(themeId);
            if (theme == null)
                theme = this.Options.SupportedThemes.FirstOrDefault(t => t.ID.Equals(this.Options.DefaultTheme));

            // Sets the theme:
            // Checks if the user is authenticated, to assign the theme to the database:
            if (_userIdentity.IsAuthenticated())
            {
                await this.SetUserThemeToDatabase(theme);
            }

            // Assigns the theme to the cookie:
            this.SetUserThemeToCookie(theme);

            // Assigns the theme to the current scope:
            this.Theme = theme;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the client's theme, either from the database (if user is registered) or the cookie.
        /// </summary>
        private void InitializeClientTheme()
        {
            // The following (3) steps are looking for a supported theme in various methods:
            Theme userTheme = null;

            // (1) - Checks if the user is authenticated - then gets the theme from the DB:
            if (_userIdentity.IsAuthenticated())
                userTheme = Task.Run(() => GetUserThemeByDatabase()).Result;

            // (2) - User is not authenticated - so gets the theme from the cookie:
            if (userTheme == null)
                userTheme = this.GetUserThemeByCookie();

            // (3) - If supported theme was not found on all methods - then sets the default:
            if (userTheme == null)
                userTheme = this.Options.SupportedThemes.FirstOrDefault(t => t.ID.Equals(this.Options.DefaultTheme));

            // Finally, assigns the theme to the cookie:
            this.SetUserThemeToCookie(userTheme);
            // Assigns the theme found to the current scope:
            this.Theme = userTheme;
        }

        /// <summary>
        /// Returns the supported theme for the specified theme id, or null if not supported.
        /// </summary>
        /// <param name="themeId">The id of the theme to get.</param>
        /// <returns>Returns the supported theme for the specified theme id, or null if not supported.</returns>
        private Theme GetSupportedThemeOrNull(string themeId)
        {
            if (string.IsNullOrEmpty(themeId))
                return null;

            foreach (Theme theme in this.Options.SupportedThemes)
            {
                if (string.Equals(theme.ID, themeId, StringComparison.OrdinalIgnoreCase))
                    return theme;
            }
            return null;
        }

        /// <summary>
        /// Gets the theme of the user by the database (for an authenticated user).
        /// </summary>
        /// <returns>Returns the theme of the user by the database if found and supported, otherwise null.</returns>
        private async Task<Theme> GetUserThemeByDatabase()
        {
            User user = await _userIdentity.GetCurrentAsync();
            if (user != null)
            {
                return this.GetSupportedThemeOrNull(user.Theme);
            }
            return null;
        }

        /// <summary>
        /// Gets the theme of the user by the cookie.
        /// </summary>
        /// <returns>Returns the theme of the user by the cookie if found and supported, otherwise null.</returns>
        private Theme GetUserThemeByCookie()
        {
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[THEME_COOKIE];
            return this.GetSupportedThemeOrNull(cookieValue);
        }

        /// <summary>
        /// Sets the specified theme to the current authenticated user in the database.
        /// </summary>
        /// <param name="theme">The theme to set to the current user.</param>
        private async Task SetUserThemeToDatabase(Theme theme)
        {
            User user = await _userIdentity.GetCurrentAsync();
            if (user != null)
            {
                await _users.UpdateUserPreferences(user.Id, user.Currency, theme.ID);
            }
        }

        /// <summary>
        /// Sets the specified theme to the current user's cookie.
        /// </summary>
        /// <param name="theme">The theme to set to the current user's cookie.</param>
        private void SetUserThemeToCookie(Theme theme)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(THEME_COOKIE, theme.ID, new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTimeOffset.Now.AddYears(100)
            });
        }

        #endregion

    }
}
