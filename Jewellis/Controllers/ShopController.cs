using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    [Route("shop")]
    public class ShopController : Controller
    {

        [Route("/shop")]
        public IActionResult Index(string category, string type)
        {
            return View();
        }

    }
}
