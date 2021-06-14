using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.App_Custom.Services.AuthUser
{
    /// <summary>
    /// Represents a service (scoped) for getting the current authenticated user info from cache memory or database.
    /// </summary>
    public class AuthUserService
    {
        #region Constants

        private const string CACHE_IDENTIFIER = AppKeys.Cache.AuthUser;

        #endregion

        #region Private Members

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JewellisDbContext _dbContext;
        private readonly IMemoryCache _cache;

        #endregion

        /// <summary>
        /// Represents a service (scoped) for getting the current authenticated user info from cache memory or database.
        /// </summary>
        public AuthUserService(IHttpContextAccessor httpContextAccessor, JewellisDbContext dbContext, IMemoryCache cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _cache = cache;
        }

        #region Public API

        /// <summary>
        /// Gets an indicator if the current user is authenticated or not.
        /// This is an extension to the default "HttpContext.User.Identity.IsAuthenticated", 
        /// by getting and checking the user id in the cookie claims.
        /// </summary>
        /// <returns>Returns true if the current user is authenticated, otherwise false.</returns>
        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.GetId().HasValue;
        }

        /// <summary>
        /// Gets the authenticated user info, either from the cache or the database.
        /// </summary>
        /// <returns>Returns the authenticated user info, either from the cache or the database if found, otherwise null.</returns>
        public async Task<User> GetAsync()
        {
            int? userId = _httpContextAccessor.HttpContext.User.Identity.GetId();
            if (userId.HasValue)
            {
                // Gets the user from cache:
                User user = this.GetUserByCache(userId.Value);
                if (user == null)
                {
                    // User not found in cache - so gets from the database:
                    user = await GetUserByDatabase(userId.Value);
                    if (user == null)
                        throw new Exception("The user id in the auth-cookie was not found in the database.");

                    this.SetUserToCache(user);
                }
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

            this.SetUserToCache(user);
        }

        /// <summary>
        /// Removes the authenticated user info from the cache memory.
        /// </summary>
        /// <param name="userId">The id of the authenticated user to remove.</param>
        public void Remove(int userId)
        {
            this.RemoveUserFromCache(userId);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the authenticated user info by the database.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <returns>Returns the authenticated user info by the database if found, otherwise null.</returns>
        private async Task<User> GetUserByDatabase(int userId)
        {
            var query = await (from user in _dbContext.Users.Include(u => u.Address)
                               where (user.Id == userId)
                               join uwp in _dbContext.UserWishlistProducts on user.Id equals uwp.UserId
                               join product in _dbContext.Products on uwp.ProductId equals product.Id
                               join sale in _dbContext.Sales on product.SaleId equals sale.Id into sj
                               from saleJoin in sj.DefaultIfEmpty()
                               select new
                               {
                                   User = user,
                                   Uwp = uwp,
                                   Product = product,
                                   Sale = saleJoin
                               }).ToListAsync();

            if (query != null && query.Count > 0)
                return query[0].User;
            else
                return null;
        }

        /// <summary>
        /// Gets the authenticated user info by the memory cache.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <returns>Returns the authenticated user info by the memory cache if found, otherwise null.</returns>
        private User GetUserByCache(int userId)
        {
            User user = null;
            if (_cache.TryGetValue($"{CACHE_IDENTIFIER}_{userId}", out user))
            {
                return user;
            }
            return null;
        }

        /// <summary>
        /// Sets the authenticated user info to the memory cache.
        /// </summary>
        /// <param name="user">The user to set.</param>
        private void SetUserToCache(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"{nameof(user)} cannot be null.");

            _cache.Set($"{CACHE_IDENTIFIER}_{user.Id}", user, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }

        /// <summary>
        /// Removes the authenticated user from the memory cache.
        /// </summary>
        /// <param name="userId">The id of the user to remove.</param>
        private void RemoveUserFromCache(int userId)
        {
            _cache.Remove($"{CACHE_IDENTIFIER}_{userId}");
        }

        #endregion

    }
}
