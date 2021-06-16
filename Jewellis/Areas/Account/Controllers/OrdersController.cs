using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly UserIdentityService _userIdentity;

        public OrdersController(JewellisDbContext dbContext, UserIdentityService userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        [Route("/account/orders")]
        public async Task<IActionResult> Index()
        {
            User user = await _userIdentity.GetCurrentAsync();
            if (user == null)
                return NotFound();



            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View();
        }

        [Route("/account/order/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            User user = await _userIdentity.GetCurrentAsync();
            if (user == null)
                return NotFound();



            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View();
        }

    }
}
