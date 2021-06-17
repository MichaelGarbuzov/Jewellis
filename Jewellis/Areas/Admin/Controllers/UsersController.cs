using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Jewellis.Areas.Admin.ViewModels.Users;
using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Services;
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
    public class UsersController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly UsersService _users;

        public UsersController(JewellisDbContext dbContext, UsersService users)
        {
            _dbContext = dbContext;
            _users = users;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<User> users = await _dbContext.Users
                .Where(u => (model.Query == null || u.FirstName.Contains(model.Query) || u.LastName.Contains(model.Query) || u.EmailAddress.Contains(model.Query) || (u.FirstName + " " + u.LastName).Equals(model.Query)) &&
                            (model.Role == null || u.Role == model.Role.Value))
                .OrderByDescending(u => u.DateRegistered)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(users.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    users = users
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["UsersModel"] = users;
            return View(model);
        }

        // GET: /Admin/Users/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            User user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            else
                return View(user);
        }

        // GET: /Admin/Users/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            User user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = user.Id,
                    CurrentEmail = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    Role = user.Role
                });
        }

        // POST: /Admin/Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            await _users.EditUser(id, model.FirstName, model.LastName, model.EmailAddress, model.Role);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            User user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            else
                return View(user);
        }

        // POST: /Admin/Users/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            await _users.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckEmailEditAvailability(string emailAddress, string currentEmail)
        {
            // Checks if the email did not change in the edit:
            if (string.Equals(emailAddress, currentEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }
            // Otherwise, email was changed so checks availability:
            else
            {
                bool isEmailAvailable = await _users.IsEmailAddressAvailable(emailAddress);
                return Json(isEmailAvailable);
            }
        }

        #endregion

    }
}
