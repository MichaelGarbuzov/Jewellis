using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {

        [Route("/cart")]
        public IActionResult ViewCart()
        {
            return View();
        }

    }
}
