using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ProductController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public ProductController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("/product/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            Product product = await _dbContext.Products
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsAvailable);
            if (product == null)
                return NotFound();
            else
                return View(product);
        }

        [Route("/product")]
        public async Task<IActionResult> Index(string name)
        {
            Product product = await _dbContext.Products
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.Name.Equals(name) && p.IsAvailable);
            if (product == null)
                return NotFound();
            else
                return View(product);
        }

    }
}
