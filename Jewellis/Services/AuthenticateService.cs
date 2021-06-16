using Jewellis.App_Custom.Helpers;
using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.App_Custom.Services.ClientTheme;
using Jewellis.App_Custom.Services.UserCache;
using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jewellis.Services
{
    /// <summary>
    /// Represents a service for authenticating users.
    /// </summary>
    public class AuthenticateService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JewellisDbContext _dbContext;
        private readonly UsersService _users;
        private readonly UserCacheService _userCache;
        private readonly ClientCurrencyService _clientCurrency;
        private readonly ClientThemeService _clientTheme;

        public AuthenticateService(IHttpContextAccessor httpContextAccessor, JewellisDbContext dbContext, UsersService users, UserCacheService userCache, ClientCurrencyService clientCurrency, ClientThemeService clientTheme)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _users = users;
            _userCache = userCache;
            _clientCurrency = clientCurrency;
            _clientTheme = clientTheme;
        }

        #region Public API

        /// <summary>
        /// Registers a new user by the specified user info parameters.
        /// </summary>
        /// <remarks>Handles: password encryption, cookie authentication, client's preferences (like currency and theme), and also user cache memory.</remarks>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="subscribeNewsletter">Indicates whether to subscribe the user to the newsletter or not.</param>
        /// <returns>Returns the user info from the database.</returns>
        public async Task<User> RegisterAsync(string firstName, string lastName, string emailAddress, string password, bool subscribeNewsletter)
        {
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException(nameof(firstName), $"{nameof(firstName)} cannot be null or empty.");
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentNullException(nameof(lastName), $"{nameof(lastName)} cannot be null or empty.");
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress), $"{nameof(emailAddress)} cannot be null or empty.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), $"{nameof(password)} cannot be null or empty.");

            // Creates a user object by the specified user info parameters:
            string passwordSalt = EncryptionHelper.GenerateSalt();
            string passwordHash = EncryptionHelper.HashSHA256(password + passwordSalt);
            User newUser = new User()
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Currency = _clientCurrency.Currency.Code,
                Theme = _clientTheme.Theme.ID,
                Role = UserRole.Customer
            };
            _dbContext.Users.Add(newUser);

            // Adds to newsletter subscription:
            if (subscribeNewsletter)
            {
                // Checks the email is not already subscribed:
                if (await _dbContext.NewsletterSubscribers.AnyAsync(s => s.EmailAddress.Equals(emailAddress)) == false)
                {
                    NewsletterSubscriber subscriber = new NewsletterSubscriber()
                    {
                        EmailAddress = emailAddress
                    };
                    _dbContext.NewsletterSubscribers.Add(subscriber);
                }
            }

            await _dbContext.SaveChangesAsync();
            User userFromDB = await _users.GetUserByEmailAsync(emailAddress);
            if (userFromDB == null)
                throw new NullReferenceException($"Error getting the user info after insertion to the database, {nameof(userFromDB)} cannot be null.");

            this.SetClientAuthentication(userFromDB, false);
            _userCache.Set(userFromDB);
            return userFromDB;
        }

        /// <summary>
        /// Logins a user by the specified email address and password.
        /// </summary>
        /// <remarks>Handles cookie authentication, and also user cache memory.</remarks>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="rememberMe">Indiactes whether or not to remember the user login for next sessions (persist the auth cookie).</param>
        /// <returns>Returns the user info from the database if login was succeeded, otherwise null.</returns>
        public async Task<User> LoginAsync(string emailAddress, string password, bool rememberMe)
        {
            User user = await _users.GetUserByEmailAsync(emailAddress);
            if (user != null)
            {
                string passwordHash = EncryptionHelper.HashSHA256(password + user.PasswordSalt);
                if (await _dbContext.Users.AnyAsync(u => u.Id == user.Id && u.PasswordHash.Equals(passwordHash)))
                {
                    this.SetClientAuthentication(user, rememberMe);
                    _userCache.Set(user);
                }
                else
                {
                    user = null;
                }
            }
            return user;
        }

        /// <summary>
        /// Logs out the current authenticated user.
        /// </summary>
        /// <remarks>Handles cookie authentication, and also user cache memory.</remarks>
        public async Task LogoutAsync()
        {
            // Removes the user from cache:
            int? userId = _httpContextAccessor.HttpContext.User.Identity.GetId();
            if (userId.HasValue)
                _userCache.Remove(userId.Value);

            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the client's authentication cookie.
        /// </summary>
        /// <param name="user">The client user to set the authentication.</param>
        /// <param name="remember">Indicates whether to remember the user authentication, or forget after session expire.</param>
        private async void SetClientAuthentication(User user, bool remember)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"{nameof(user)} cannot be null.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = remember
            };
            if (remember)
            {
                authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(365);
            }

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        #endregion

    }
}
