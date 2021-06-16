using Microsoft.Extensions.DependencyInjection;

namespace Jewellis.App_Custom.Services.UserCache
{
    /// <summary>
    /// Represents extension methods for the <see cref="IServiceCollection"/> to add the <see cref="UserCacheService"/>.
    /// </summary>
    public static class UserCacheServiceExtensions
    {

        /// <summary>
        /// Adds a custom service (transient) for caching authenticated users to the cache memory.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns>Returns the services collections.</returns>
        public static IServiceCollection AddUserCache(this IServiceCollection services)
        {
            return services.AddTransient<UserCacheService>();
        }

    }
}
