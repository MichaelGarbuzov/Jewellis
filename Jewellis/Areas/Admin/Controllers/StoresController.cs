using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Jewellis.Areas.Admin.ViewModels.Stores;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StoresController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public StoresController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/Stores
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<Branch> branches = await _dbContext.Branches
                .Where(b => (model.Query == null) || b.Name.Contains(model.Query))
                .OrderBy(b => b.Name)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(branches.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    branches = branches
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["BranchesModel"] = branches;
            return View(model);
        }

        // GET: /Admin/Stores/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Branch branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
                return NotFound();
            else
                return View(branch);
        }

        // GET: /Admin/Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Stores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Branch branch = new Branch()
            {
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                OpeningHours = model.OpeningHours,
                LocationLatitude = model.LocationLatitude,
                LocationLongitude = model.LocationLongitude
            };
            _dbContext.Branches.Add(branch);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Stores/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Branch branch = await _dbContext.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = branch.Id,
                    CurrentName = branch.Name,
                    Name = branch.Name,
                    Address = branch.Address,
                    PhoneNumber = branch.PhoneNumber,
                    OpeningHours = branch.OpeningHours,
                    LocationLatitude = branch.LocationLatitude,
                    LocationLongitude = branch.LocationLongitude
                });
        }

        // POST: /Admin/Stores/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            Branch branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
                return NotFound();

            // Binds the view model:
            branch.Name = model.Name;
            branch.Address = model.Address;
            branch.PhoneNumber = model.PhoneNumber;
            branch.OpeningHours = model.OpeningHours;
            branch.LocationLatitude = model.LocationLatitude;
            branch.LocationLongitude = model.LocationLongitude;
            branch.DateLastModified = DateTime.Now;

            _dbContext.Branches.Update(branch);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Stores/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Branch branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
                return NotFound();
            else
                return View(branch);
        }

        // POST: /Admin/Stores/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Branch branch = await _dbContext.Branches.FindAsync(id);
            _dbContext.Branches.Remove(branch);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.Branches.AnyAsync(b => b.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.Branches.AnyAsync(b => b.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
