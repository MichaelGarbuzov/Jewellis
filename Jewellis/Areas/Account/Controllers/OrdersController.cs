using Jewellis.App_Custom.Services.AuthUser;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly AuthUserService _authUser;

        public OrdersController(JewellisDbContext dbContext, AuthUserService authUser)
        {
            _dbContext = dbContext;
            _authUser = authUser;
        }

        [Route("/account/orders")]
        public async Task<IActionResult> Index()
        {
            User user = await _authUser.GetAsync();
            if (user == null)
                return NotFound();



            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View();
        }

        [Route("/account/order/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            User user = await _authUser.GetAsync();
            if (user == null)
                return NotFound();



            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View();
        }

    }
}
