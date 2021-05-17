using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    [Route("order")]
    public class OrderController : Controller
    {

        [Route("/checkout")]
        public IActionResult Checkout()
        {
            return View();
        }

        [Route("/checkout-confirmed")]
        public IActionResult CheckoutConfirmed()
        {
            return View();
        }

    }
}
