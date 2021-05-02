namespace Jewellis.App_Custom.Services.ClientTheme
{
    /// <summary>
    /// Represents the options to configure the <see cref="ClientThemeService"/>.
    /// </summary>
    public class ClientThemeOptions
    {

        /// <summary>
        /// Gets or sets the default theme ID (<see cref="Theme.ID"/>).
        /// </summary>
        public string DefaultTheme { get; set; }

        /// <summary>
        /// Gets or sets the list of supported client themes in the application.
        /// </summary>
        public Theme[] SupportedThemes { get; set; }

    }
}
