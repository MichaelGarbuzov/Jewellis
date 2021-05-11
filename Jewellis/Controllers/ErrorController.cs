using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    public class ErrorController : Controller
    {

        public IActionResult HttpError404()
        {
            return View();
        }

        public IActionResult HttpError500()
        {
            return View();
        }

    }
}
