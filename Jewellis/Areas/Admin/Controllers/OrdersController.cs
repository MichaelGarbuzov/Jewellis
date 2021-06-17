using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Jewellis.Areas.Admin.ViewModels.Orders;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Jewellis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly OrdersService _orders;

        public OrdersController(OrdersService orders)
        {
            _orders = orders;
        }

        // GET: /Admin/Orders
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<Order> orders = await _orders.Search(model.Status, model.DateCreated, model.OrderId);

            #region Pagination...

            Pagination pagination = new Pagination(orders.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    orders = orders
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["OrdersModel"] = orders;
            return View(model);
        }

        // GET: /Admin/Orders/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Order order = await _orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            else
                return View(order);
        }

        // GET: /Admin/Orders/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Order order = await _orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            else
                return View(order);
        }

        // POST: /Admin/Orders/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            await _orders.DeleteOrder(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Orders/IndexUpdateStatus/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexUpdateStatus(int id, OrderStatus status)
        {
            await _orders.UpdateStatus(id, status);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Orders/DetailsUpdateStatus/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsUpdateStatus(int id, OrderStatus status)
        {
            await _orders.UpdateStatus(id, status);
            return RedirectToAction(nameof(Details), new { id = id });
        }

    }
}
