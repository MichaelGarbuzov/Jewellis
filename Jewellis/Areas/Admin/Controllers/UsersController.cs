using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Services.AuthUser;
using Jewellis.Areas.Admin.ViewModels.Users;
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
    public class UsersController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly AuthUserService _authUser;

        public UsersController(JewellisDbContext dbContext, AuthUserService authUser)
        {
            _dbContext = dbContext;
            _authUser = authUser;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<User> users = await _dbContext.Users
                .Where(u => (model.Query == null || u.FirstName.Contains(model.Query) || u.LastName.Contains(model.Query) || u.EmailAddress.Contains(model.Query) || (u.FirstName + " " + u.LastName).Equals(model.Query)) &&
                            (model.Role == null || u.Role == model.Role.Value))
                .OrderByDescending(u => u.DateRegistered)
                .ToListAsync();
            ViewData["UsersModel"] = users;
            return View(model);
        }

        // GET: /Admin/Users/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            User user = await _dbContext.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();
            else
                return View(user);
        }

        // GET: /Admin/Users/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            User user = await _dbContext.Users.FindAsync(id);
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

            User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            // Binds the view model:
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.EmailAddress = model.EmailAddress;
            user.Role = model.Role;
            user.DateLastModified = DateTime.Now;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            _authUser.Set(user);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            User user = await _dbContext.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == id);
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
            User user = await _dbContext.Users.FindAsync(id);
            if (user.ClientCartId.HasValue)
            {
                ClientCart cart = await _dbContext.ClientCarts.FindAsync(user.ClientCartId.Value);
                _dbContext.ClientCarts.Remove(cart);
            }
            _dbContext.Addresses.Remove(user.Address);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            _authUser.Remove(user.Id);
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
                bool isEmailAvailable = (await _dbContext.Users.AnyAsync(u => u.EmailAddress.Equals(emailAddress)) == false);
                return Json(isEmailAvailable);
            }
        }

        #endregion

    }
}
