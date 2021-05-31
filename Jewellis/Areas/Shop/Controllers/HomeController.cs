using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class HomeController : Controller
    {

        [Route("/shop")]
        public IActionResult Index(string category, string type)
        {
            return View();
        }

    }
}
