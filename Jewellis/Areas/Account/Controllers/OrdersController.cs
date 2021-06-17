using Jewellis.Models;
using Jewellis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly UserIdentityService _userIdentity;
        private readonly UsersService _users;
        private readonly OrdersService _orders;

        public OrdersController(UserIdentityService userIdentity, UsersService users, OrdersService orders)
        {
            _userIdentity = userIdentity;
            _users = users;
            _orders = orders;
        }

        [Route("/account/orders")]
        public async Task<IActionResult> Index()
        {
            User user = await _userIdentity.GetCurrentAsync();
            if (user == null)
                return NotFound();

            List<Order> orders = await _users.GetOrdersAsync(user.Id);

            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View(orders);
        }

        [Route("/account/order/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            User user = await _userIdentity.GetCurrentAsync();
            if (user == null)
                return NotFound();

            Order order = await _orders.GetByIdAsync(id);

            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View(order);
        }

    }
}
