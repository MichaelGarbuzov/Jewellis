using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ProductController : Controller
    {

        [Route("/product/{id}")]
        public IActionResult Index(int id)
        {
            return View();
        }

    }
}
