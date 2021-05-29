using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("shop")]
    [Route("cart")]
    public class CartController : Controller
    {

        [Route("/cart")]
        public IActionResult Index()
        {
            return View();
        }

    }
}
