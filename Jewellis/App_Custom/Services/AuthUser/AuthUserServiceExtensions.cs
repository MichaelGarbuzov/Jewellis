using Microsoft.Extensions.DependencyInjection;

namespace Jewellis.App_Custom.Services.AuthUser
{
    /// <summary>
    /// Represents extension methods for the <see cref="IServiceCollection"/> to add the <see cref="AuthUserService"/>.
    /// </summary>
    public static class AuthUserServiceExtensions
    {

        /// <summary>
        /// Adds a custom service (scoped) to get the current authenticated user info from cache memory or database.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns>Returns the services collections.</returns>
        public static IServiceCollection AddAuthUser(this IServiceCollection services)
        {
            return services.AddScoped<AuthUserService>();
        }

    }
}
