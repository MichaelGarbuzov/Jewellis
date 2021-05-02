using System;

namespace Jewellis.App_Custom.Services.ClientTheme
{
    /// <summary>
    /// Represents a supported theme in the application.
    /// </summary>
    public class Theme
    {
        #region Properties

        /// <summary>
        /// Gets the ID of the theme.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the display name of the theme.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the unique cookie value that represents this theme.
        /// </summary>
        public string CookieValue { get; private set; }

        #endregion

        /// <summary>
        /// Represents a supported theme in the application.
        /// </summary>
        /// <param name="id">The id of the theme.</param>
        /// <param name="cookieValue">The unique cookie value that represents this theme.</param>
        /// <param name="displayName">The display name of the theme.</param>
        public Theme(string id, string cookieValue, string displayName)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("{id} cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(cookieValue))
                throw new ArgumentNullException("{cookieValue} cannot be null or empty.");

            this.ID = id;
            this.DisplayName = displayName;
            this.CookieValue = cookieValue;
        }

    }
}
