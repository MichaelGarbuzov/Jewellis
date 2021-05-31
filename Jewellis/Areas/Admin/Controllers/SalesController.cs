using Jewellis.App_Custom.ActionFilters;
using Jewellis.Areas.Admin.ViewModels.Sales;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SalesController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public SalesController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/Sales
        public async Task<IActionResult> Index(string query)
        {
            List<Sale> sales = await _dbContext.Sales.Where(s => (query == null) || s.Name.Contains(query)).OrderByDescending(s => s.DateLastModified).ToListAsync();
            ViewData["SearchQuery"] = query;
            return View(sales);
        }

        // GET: /Admin/Sales/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Sale sale = await _dbContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
                return NotFound();
            else
                return View(sale);
        }

        // GET: /Admin/Sales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Sale sale = new Sale()
            {
                Name = model.Name,
                DiscountRate = model.DiscountRate,
                DateStart = model.DateStart,
                DateEnd = model.DateEnd
            };
            _dbContext.Sales.Add(sale);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Sales/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Sale sale = await _dbContext.Sales.FindAsync(id);
            if (sale == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = sale.Id,
                    CurrentName = sale.Name,
                    Name = sale.Name,
                    DiscountRate = sale.DiscountRate,
                    DateStart = sale.DateStart,
                    DateEnd = sale.DateEnd
                });
        }

        // POST: /Admin/Sales/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            Sale sale = await _dbContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
                return NotFound();

            // Binds the view model:
            sale.Name = model.Name;
            sale.DiscountRate = model.DiscountRate;
            sale.DateStart = model.DateStart;
            sale.DateEnd = model.DateEnd;
            sale.DateLastModified = DateTime.Now;

            _dbContext.Sales.Update(sale);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Sales/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Sale sale = await _dbContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
                return NotFound();
            else
                return View(sale);
        }

        // POST: /Admin/Sales/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Sale sale = await _dbContext.Sales.FindAsync(id);
            _dbContext.Sales.Remove(sale);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.Sales.AnyAsync(s => s.Name.Equals(name)) == false);
            return Json(isNameAvailable);
        }

        [AjaxOnly]
        public async Task<JsonResult> CheckNameEditAvailability(string name, string currentName)
        {
            // Checks if the name did not change in the edit:
            if (string.Equals(name, currentName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }
            // Otherwise, name was changed so checks availability:
            else
            {
                bool isNameAvailable = (await _dbContext.Sales.AnyAsync(s => s.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
