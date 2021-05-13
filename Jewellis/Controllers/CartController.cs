using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {

        [Route("view")]
        public IActionResult ViewCart()
        {
            return View();
        }

        [Route("checkout")]
        public IActionResult Checkout()
        {
            return View();
        }

    }
}
