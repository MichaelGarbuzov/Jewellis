using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Helpers;
using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.App_Custom.Services.ClientTheme;
using Jewellis.Areas.Account.ViewModels.Home;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly ClientCurrencyService _clientCurrency;
        private readonly ClientThemeService _clientTheme;

        public HomeController(JewellisDbContext dbContext, ClientCurrencyService clientCurrency, ClientThemeService clientTheme)
        {
            _dbContext = dbContext;
            _clientCurrency = clientCurrency;
            _clientTheme = clientTheme;
        }

        [Route("/account/orders")]
        public async Task<IActionResult> Orders()
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();



            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View();
        }

        [Route("/account/order/{id}")]
        public async Task<IActionResult> Order(int id)
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();



            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View();
        }

        [Route("/account/wishlist")]
        public async Task<IActionResult> Wishlist()
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();



            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View();
        }

        [Route("/account/profile")]
        public async Task<IActionResult> Profile()
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();

            ProfileVM model = new ProfileVM()
            {
                EditProfileVM = new ProfileVM.EditProfileSubVM()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    CurrentEmail = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber
                },
                EditPasswordVM = new ProfileVM.EditPasswordSubVM(),
                EditPreferencesVM = new ProfileVM.EditPreferencesSubVM()
                {
                    Currency = user.Currency,
                    Theme = user.Theme
                }
            };
            ViewData["SupportedCurrencies"] = new SelectList(_clientCurrency.Options.SupportedCurrencies, nameof(Currency.Code), nameof(Currency.Code));
            ViewData["SupportedThemes"] = new SelectList(_clientTheme.Options.SupportedThemes, nameof(Theme.ID), nameof(Theme.DisplayName));
            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileVM model)
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(nameof(Profile), model);

            // Binds the view model:
            user.FirstName = model.EditProfileVM.FirstName;
            user.LastName = model.EditProfileVM.LastName;
            user.EmailAddress = model.EditProfileVM.EmailAddress;
            user.PhoneNumber = model.EditProfileVM.PhoneNumber;
            user.DateLastModified = DateTime.Now;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessTitle"] = "Profile Updated";
            TempData["SuccessBody"] = "Your profile has been updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(ProfileVM model)
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(nameof(Profile), model);

            // First, checks the current password is correct:
            string currentPasswordHash = EncryptionHelper.HashSHA256(model.EditPasswordVM.CurrentPassword + user.PasswordSalt);
            user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.PasswordHash.Equals(currentPasswordHash));
            if (user != null)
            {
                // Binds the view model:
                user.PasswordSalt = EncryptionHelper.GenerateSalt();
                user.PasswordHash = EncryptionHelper.HashSHA256(model.EditPasswordVM.NewPassword + user.PasswordSalt);
                user.Address.DateLastModified = DateTime.Now;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessTitle"] = "Password Changed";
                TempData["SuccessBody"] = "Your password has been changed successfully.";
                return RedirectToAction(nameof(Profile));
            }
            else
            {
                TempData["EditPassword_ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction(nameof(Profile));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPreferences(ProfileVM model)
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(nameof(Profile), model);

            // Binds the view model:
            await _clientCurrency.SetAsync(model.EditPreferencesVM.Currency);
            await _clientTheme.SetAsync(model.EditPreferencesVM.Theme);

            TempData["SuccessTitle"] = "Preferences Updated";
            TempData["SuccessBody"] = "Your preferences has been updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        [Route("/account/address")]
        public async Task<IActionResult> Address()
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                AddressVM model = new AddressVM();
                if (user.Address != null)
                {
                    model.Street = user.Address.Street;
                    model.PostalCode = user.Address.PostalCode;
                    model.City = user.Address.City;
                    model.Country = user.Address.Country;
                }
                ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(AddressVM model)
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(nameof(Address), model);

            // Checks if the user has no address yet:
            if (user.Address == null)
                user.Address = new Address();

            // Binds the view model:
            user.Address.Street = model.Street;
            user.Address.PostalCode = model.PostalCode;
            user.Address.City = model.City;
            user.Address.Country = model.Country;
            user.Address.DateLastModified = DateTime.Now;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessTitle"] = "Address Updated";
            TempData["SuccessBody"] = "Your address has been updated successfully.";
            return RedirectToAction(nameof(Address));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress()
        {
            User user = await this.GetUserByAuthInfo();
            if (user == null)
                return NotFound();

            if (user.Address != null)
            {
                user.AddressId = null;
                _dbContext.Users.Update(user);
                _dbContext.Addresses.Remove(user.Address);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessTitle"] = "Address Removed";
                TempData["SuccessBody"] = "Your address has been removed successfully.";
            }
            return RedirectToAction(nameof(Address));
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

        #region Private Methods

        /// <summary>
        /// Gets the user from the database by the authentication information.
        /// </summary>
        /// <returns>Returns the user from the database by the authentication information if found, otherwise null.</returns>
        private async Task<User> GetUserByAuthInfo()
        {
            int? userId = HttpContext.User.Identity.GetId();
            if (userId != null)
            {
                User user = await _dbContext.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user != null)
                    return user;
            }
            return null;
        }

        #endregion

    }
}
