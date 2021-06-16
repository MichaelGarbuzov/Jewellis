using Jewellis.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Jewellis.Services
{
    /// <summary>
    /// Represents a service for getting the current authenticated user's info.
    /// </summary>
    public class UserIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsersService _users;

        public UserIdentityService(IHttpContextAccessor httpContextAccessor, UsersService users)
        {
            _httpContextAccessor = httpContextAccessor;
            _users = users;
        }

        #region Public API

        /// <summary>
        /// Checks whether the current user is authenticated or not.
        /// </summary>
        /// <remarks>This is an extension to the "HttpContext.User.Identity.IsAuthenticated" that checks the user id in the claims of the auth cookie too.</remarks>
        /// <returns>Returns true if the current user is authenticated, otherwise false.</returns>
        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.GetId().HasValue;
        }

        /// <summary>
        /// Gets the id of the current authenticated user.
        /// </summary>
        /// <returns>Returns the id of the current authenticated user if found, otherwise null.</returns>
        public int? GetCurrentId()
        {
            return _httpContextAccessor.HttpContext.User.Identity.GetId();
        }

        /// <summary>
        /// Gets the current authenticated user info, either from the cache or the database.
        /// </summary>
        /// <returns>Returns the current authenticated user info, either from the cache or the database if found, otherwise null.</returns>
        public async Task<User> GetCurrentAsync()
        {
            int? userId = this.GetCurrentId();
            if (userId.HasValue)
                return await _users.GetByIdAsync(userId.Value);
            else
                return null;
        }

        #endregion

    }
}
