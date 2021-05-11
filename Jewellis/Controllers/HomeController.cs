using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("terms")]
        public IActionResult Terms()
        {
            return View();
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("help")]
        public IActionResult Help()
        {
            return View();
        }

    }
}
