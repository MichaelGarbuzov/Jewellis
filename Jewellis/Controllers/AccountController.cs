using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    [Route("account")]
    public class AccountController : Controller
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

        [Route("logout")]
        public IActionResult Logout()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Route("orders")]
        public IActionResult Orders()
        {
            return View();
        }

        [Route("wishlist")]
        public IActionResult Wishlist()
        {
            return View();
        }

        [Route("profile")]
        public IActionResult Profile()
        {
            return View();
        }

        [Route("address")]
        public IActionResult Address()
        {
            return View();
        }

    }
}
