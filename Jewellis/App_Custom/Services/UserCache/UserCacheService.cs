using Jewellis.Models;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Jewellis.App_Custom.Services.UserCache
{
    /// <summary>
    /// Represents a service (transient) for caching authenticated users to the cache memory.
    /// </summary>
    public class UserCacheService
    {
        private const string CACHE_IDENTIFIER = AppKeys.Cache.User;

        private readonly IMemoryCache _cache;

        /// <summary>
        /// Represents a service (transient) for caching authenticated users to the cache memory.
        /// </summary>
        public UserCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        #region Public API

        /// <summary>
        /// Gets the authenticated user info by the memory cache.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <returns>Returns the authenticated user info by the memory cache if found, otherwise null.</returns>
        public User Get(int userId)
        {
            User user = null;
            if (_cache.TryGetValue($"{CACHE_IDENTIFIER}_{userId}", out user))
            {
                return user;
            }
            return null;
        }

        /// <summary>
        /// Sets/Updates the authenticated user info in the cache memory.
        /// </summary>
        /// <param name="user">The authenticated user info to set.</param>
        public void Set(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"{nameof(user)} cannot be null.");

            _cache.Set($"{CACHE_IDENTIFIER}_{user.Id}", user, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }

        /// <summary>
        /// Removes the authenticated user info from the cache memory.
        /// </summary>
        /// <param name="userId">The id of the authenticated user to remove.</param>
        public void Remove(int userId)
        {
            _cache.Remove($"{CACHE_IDENTIFIER}_{userId}");
        }

        #endregion

    }
}
