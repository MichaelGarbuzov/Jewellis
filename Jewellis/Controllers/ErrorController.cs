using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    public class ErrorController : Controller
    {

        [Route("/Error/ExceptionHandler")]
        public IActionResult ExceptionHandler()
        {
            var exceptionData = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // Checks if no error has been thrown (if someone navigates to this action accidentally):
            if (exceptionData == null)
            {
                return NotFound();
            }
            else
            {
                // Here, an error has been thrown:
                Response.StatusCode = 500;
                return View("Default");
            }
        }

        [Route("/Error/StatusCodeHandler")]
        public IActionResult StatusCodeHandler()
        {
            var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // Checks if no error has been thrown (if someone navigates to this action accidentally):
            if (statusFeature == null)
            {
                return NotFound();
            }
            else
            {
                // Here, a status code has been thrown:
                switch (Response.StatusCode)
                {
                    case 404:
                        return View("HttpError404");
                    case 400:
                        // Status "400 (Bad Request)" is OK to return:
                        return View("Default");
                    default:
                        // Status codes other than "500 (Server Error)" are not allowed:
                        Response.StatusCode = 500;
                        return View("Default");
                }
            }
        }

        [Route("/error/access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
