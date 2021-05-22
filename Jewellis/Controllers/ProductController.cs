using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Controllers
{
    [Route("product")]
    public class ProductController : Controller
    {

        [Route("{id}")]
        public IActionResult Read(int id)
        {
            return View();
        }

    }
}

