using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {

        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("wishlist")]
        public IActionResult Wishlist()
        {
            return View();
        }

    }
}
