using Microsoft.Extensions.DependencyInjection;
using System;

namespace Jewellis.App_Custom.Services.ClientTheme
{
    /// <summary>
    /// Represents extension methods for the <see cref="IServiceCollection"/> to add the <see cref="ClientThemeService"/>.
    /// </summary>
    public static class ClientThemeServiceExtensions
    {

        /// <summary>
        /// Adds a custom service (scoped) to manage the client's theme UI.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <param name="configureOptions">A delegate to configure the options.</param>
        /// <returns>Returns the services collections.</returns>
        public static IServiceCollection AddClientTheme(this IServiceCollection services, Action<ClientThemeOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddScoped<ClientThemeService>();
        }

    }
}
