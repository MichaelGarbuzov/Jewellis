using Microsoft.Extensions.DependencyInjection;
using System;

namespace Jewellis.App_Custom.Services.ClientCurrency
{
    /// <summary>
    /// Represents extension methods for the <see cref="IServiceCollection"/> to add the <see cref="ClientCurrencyService"/>.
    /// </summary>
    public static class ClientCurrencyServiceExtensions
    {

        /// <summary>
        /// Adds a custom service (scoped) to manage the client's currency.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <param name="configureOptions">A delegate to configure the options.</param>
        /// <returns>Returns the services collections.</returns>
        public static IServiceCollection AddClientCurrency(this IServiceCollection services, Action<ClientCurrencyOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddScoped<ClientCurrencyService>();
        }

    }
}
