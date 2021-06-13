using Microsoft.Extensions.DependencyInjection;

namespace Jewellis.App_Custom.Services.ClientShoppingCart
{
    /// <summary>
    /// Represents extension methods for the <see cref="IServiceCollection"/> to add the <see cref="ClientShoppingCartService"/>.
    /// </summary>
    public static class ClientShoppingCartServiceExtensions
    {

        /// <summary>
        /// Adds a custom service (scoped) to manage the client's shopping cart.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns>Returns the services collections.</returns>
        public static IServiceCollection AddClientShoppingCart(this IServiceCollection services)
        {
            return services.AddScoped<ClientShoppingCartService>();
        }

    }
}
