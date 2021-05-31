using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}
