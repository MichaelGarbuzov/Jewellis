using Jewellis.App_Custom.ActionFilters;
using Jewellis.Areas.Admin.ViewModels.ProductCategories;
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
    public class ProductCategoriesController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public ProductCategoriesController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/ProductCategories
        public async Task<IActionResult> Index(string query)
        {
            List<ProductCategory> categories = await _dbContext.ProductCategories.Where(c => (query == null) || c.Name.Contains(query)).OrderBy(c => c.Name).ToListAsync();
            ViewData["SearchQuery"] = query;
            return View(categories);
        }

        // GET: /Admin/ProductCategories/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            else
                return View(category);
        }

        // GET: /Admin/ProductCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ProductCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ProductCategory category = new ProductCategory()
            {
                Name = model.Name
            };
            _dbContext.ProductCategories.Add(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ProductCategories/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FindAsync(id);
            if (category == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = category.Id,
                    CurrentName = category.Name,
                    Name = category.Name
                });
        }

        // POST: /Admin/ProductCategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            ProductCategory category = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            // Binds the view model:
            category.Name = model.Name;
            category.DateLastModified = DateTime.Now;

            _dbContext.ProductCategories.Update(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ProductCategories/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            else
                return View(category);
        }

        // POST: /Admin/ProductCategories/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FindAsync(id);
            _dbContext.ProductCategories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.ProductCategories.AnyAsync(c => c.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.ProductCategories.AnyAsync(c => c.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
