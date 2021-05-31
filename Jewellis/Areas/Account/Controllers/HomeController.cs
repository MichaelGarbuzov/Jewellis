using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    public class HomeController : Controller
    {

        [Route("/account/orders")]
        public IActionResult Orders()
        {
            return View();
        }

        [Route("/account/order/{id}")]
        public IActionResult Order(int id)
        {
            return View();
        }

        [Route("/account/wishlist")]
        public IActionResult Wishlist()
        {
            return View();
        }

        [Route("/account/profile")]
        public IActionResult Profile()
        {
            return View();
        }

        [Route("/account/address")]
        public IActionResult Address()
        {
            return View();
        }

    }
}
