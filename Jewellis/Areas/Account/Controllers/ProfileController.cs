using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.App_Custom.Services.ClientTheme;
using Jewellis.Areas.Account.ViewModels.Profile;
using Jewellis.Models;
using Jewellis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserIdentityService _userIdentity;
        private readonly UsersService _users;
        private readonly ClientCurrencyService _clientCurrency;
        private readonly ClientThemeService _clientTheme;

        public ProfileController(UserIdentityService userIdentity, UsersService users, ClientCurrencyService clientCurrency, ClientThemeService clientTheme)
        {
            _userIdentity = userIdentity;
            _users = users;
            _clientCurrency = clientCurrency;
            _clientTheme = clientTheme;
        }

        [Route("/account/profile")]
        public async Task<IActionResult> Index()
        {
            User user = await _userIdentity.GetCurrentAsync();
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
            if (!ModelState.IsValid)
                return View(nameof(Index), model);

            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            await _users.UpdateUserProfile(userId.Value, model.EditProfileVM.FirstName, model.EditProfileVM.LastName, model.EditProfileVM.EmailAddress, model.EditProfileVM.PhoneNumber);

            TempData["SuccessTitle"] = "Profile Updated";
            TempData["SuccessBody"] = "Your profile has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(ProfileVM model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), model);

            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            if (await _users.UpdateUserPassword(userId.Value, model.EditPasswordVM.CurrentPassword, model.EditPasswordVM.NewPassword))
            {
                TempData["SuccessTitle"] = "Password Changed";
                TempData["SuccessBody"] = "Your password has been changed successfully.";
            }
            else
            {
                TempData["EditPassword_ErrorMessage"] = "Current password is incorrect.";

            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPreferences(ProfileVM model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), model);

            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            await _users.UpdateUserPreferences(userId.Value, model.EditPreferencesVM.Currency, model.EditPreferencesVM.Theme);

            TempData["SuccessTitle"] = "Preferences Updated";
            TempData["SuccessBody"] = "Your preferences has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Route("/account/address")]
        public async Task<IActionResult> Address()
        {
            User user = await _userIdentity.GetCurrentAsync();
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
            if (!ModelState.IsValid)
                return View(nameof(Address), model);

            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            await _users.UpdateUserAddress(userId.Value, model.Street, model.PostalCode, model.City, model.Country);

            TempData["SuccessTitle"] = "Address Updated";
            TempData["SuccessBody"] = "Your address has been updated successfully.";
            return RedirectToAction(nameof(Address));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress()
        {
            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            if (await _users.RemoveUserAddress(userId.Value))
            {
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
                bool isEmailAvailable = await _users.IsEmailAddressAvailable(emailAddress);
                return Json(isEmailAvailable);
            }
        }

        #endregion

    }
}
