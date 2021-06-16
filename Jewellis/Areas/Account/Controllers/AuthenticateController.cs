using Jewellis.App_Custom.ActionFilters;
using Jewellis.Areas.Account.ViewModels.Authenticate;
using Jewellis.Models;
using Jewellis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    public class AuthenticateController : Controller
    {
        private readonly AuthenticateService _authenticateService;
        private readonly UsersService _users;

        public AuthenticateController(AuthenticateService authenticateService, UsersService users)
        {
            _authenticateService = authenticateService;
            _users = users;
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

            await _authenticateService.RegisterAsync(model.FirstName, model.LastName, model.EmailAddress, model.Password, model.SubscribeNewsletter);
            return this.RedirectToHomePage();
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

            User user = await _authenticateService.LoginAsync(model.EmailAddress, model.Password, model.RememberMe);
            if (user != null)
            {
                return this.RedirectToHomePage();
            }
            else
            {
                ViewData["ErrorMessage"] = "Incorrect email address or password.";
                return View(model);
            }
        }

        [Route("/account/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authenticateService.LogoutAsync();
            return this.RedirectToHomePage();
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckEmailAvailability(string emailAddress)
        {
            bool isEmailAvailable = await _users.IsEmailAddressAvailable(emailAddress);
            return Json(isEmailAvailable);
        }

        #endregion

    }
}
