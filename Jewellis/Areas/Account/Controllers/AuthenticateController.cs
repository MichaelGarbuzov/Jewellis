using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Helpers;
using Jewellis.App_Custom.Services.AuthUser;
using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.App_Custom.Services.ClientTheme;
using Jewellis.Areas.Account.ViewModels.Authenticate;
using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    public class AuthenticateController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly AuthUserService _authUser;
        private readonly ClientCurrencyService _clientCurrency;
        private readonly ClientThemeService _clientTheme;

        public AuthenticateController(JewellisDbContext dbContext, AuthUserService authUser, ClientCurrencyService clientCurrency, ClientThemeService clientTheme)
        {
            _dbContext = dbContext;
            _authUser = authUser;
            _clientCurrency = clientCurrency;
            _clientTheme = clientTheme;
        }

        [Route("/register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _dbContext.Users.AnyAsync(u => u.EmailAddress.Equals(model.EmailAddress)))
            {
                ViewData["ErrorMessage"] = "The email address is already registered.";
                return View(model);
            }

            // Registers the user:
            string passwordSalt = EncryptionHelper.GenerateSalt();
            string passwordHash = EncryptionHelper.HashSHA256(model.Password + passwordSalt);
            User newUser = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Currency = _clientCurrency.Currency.Code,
                Theme = _clientTheme.Theme.ID,
                Role = UserRole.Customer
            };
            _dbContext.Users.Add(newUser);

            // Adds to newsletter subscription:
            if (model.SubscribeNewsletter)
            {
                // Checks the email is not already subscribed:
                if (await _dbContext.NewsletterSubscribers.AnyAsync(s => s.EmailAddress.Equals(model.EmailAddress)) == false)
                {
                    NewsletterSubscriber subscriber = new NewsletterSubscriber()
                    {
                        EmailAddress = model.EmailAddress
                    };
                    _dbContext.NewsletterSubscribers.Add(subscriber);
                }
            }

            await _dbContext.SaveChangesAsync();
            User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress.Equals(newUser.EmailAddress));
            if (user != null)
            {
                this.SetClientAuthentication(user, false);
                _authUser.Set(user);
                return this.RedirectToHomePage();
            }
            else
            {
                throw new NullReferenceException($"Error getting the user info after insertion to the database, {nameof(user)} cannot be null.");
            }
        }

        [Route("/login")]
        public IActionResult Login()
        {
            return View(new LoginVM()
            {
                RememberMe = true
            });
        }

        [Route("/login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress.Equals(model.EmailAddress));
            if (user != null)
            {
                string passwordHash = EncryptionHelper.HashSHA256(model.Password + user.PasswordSalt);
                user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.PasswordHash.Equals(passwordHash));
                if (user != null)
                {
                    this.SetClientAuthentication(user, model.RememberMe);
                    _authUser.Set(user);
                    return this.RedirectToHomePage();
                }
            }
            ViewData["ErrorMessage"] = "Incorrect email address or password.";
            return View(model);
        }

        [Route("/account/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Removes the user from cache:
            int? userId = HttpContext.User.Identity.GetId();
            if (userId.HasValue)
                _authUser.Remove(userId.Value);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToHomePage();
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckEmailAvailability(string emailAddress)
        {
            bool isEmailAvailable = (await _dbContext.Users.AnyAsync(u => u.EmailAddress.Equals(emailAddress)) == false);
            return Json(isEmailAvailable);
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
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        #endregion

    }
}
