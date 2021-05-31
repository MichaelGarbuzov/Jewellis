﻿using Jewellis.App_Custom.ActionFilters;
using Jewellis.Areas.Admin.ViewModels.DeliveryMethods;
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
    public class DeliveryMethodsController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public DeliveryMethodsController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/DeliveryMethods
        public async Task<IActionResult> Index(string query)
        {
            List<DeliveryMethod> deliveryMethods = await _dbContext.DeliveryMethods.Where(d => (query == null) || d.Name.Contains(query) || d.Description.Contains(query)).OrderBy(d => d.Price).ToListAsync();
            ViewData["SearchQuery"] = query;
            return View(deliveryMethods);
        }

        // GET: /Admin/DeliveryMethods/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
            if (deliveryMethod == null)
                return NotFound();
            else
                return View(deliveryMethod);
        }

        // GET: /Admin/DeliveryMethods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/DeliveryMethods/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            DeliveryMethod deliveryMethod = new DeliveryMethod()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price
            };
            _dbContext.DeliveryMethods.Add(deliveryMethod);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/DeliveryMethods/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FindAsync(id);
            if (deliveryMethod == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = deliveryMethod.Id,
                    CurrentName = deliveryMethod.Name,
                    Name = deliveryMethod.Name,
                    Description = deliveryMethod.Description,
                    Price = deliveryMethod.Price
                });
        }

        // POST: /Admin/DeliveryMethods/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
            if (deliveryMethod == null)
                return NotFound();

            // Binds the view model:
            deliveryMethod.Name = model.Name;
            deliveryMethod.Description = model.Description;
            deliveryMethod.Price = model.Price;
            deliveryMethod.DateLastModified = DateTime.Now;

            _dbContext.DeliveryMethods.Update(deliveryMethod);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/DeliveryMethods/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
            if (deliveryMethod == null)
                return NotFound();
            else
                return View(deliveryMethod);
        }

        // POST: /Admin/DeliveryMethods/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FindAsync(id);
            _dbContext.DeliveryMethods.Remove(deliveryMethod);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.DeliveryMethods.AnyAsync(d => d.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.DeliveryMethods.AnyAsync(d => d.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}