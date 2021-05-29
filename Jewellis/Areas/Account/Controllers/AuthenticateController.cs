using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("account")]
    [Route("auth")]
    public class AuthenticateController : Controller
    {

        [Route("/register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/account/logout")]
        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Jewellis.Controllers.HomeController.Index), "Home");
        }

    }
}
