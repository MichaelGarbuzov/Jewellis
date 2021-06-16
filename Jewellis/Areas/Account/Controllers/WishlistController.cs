using Jewellis.App_Custom.ActionFilters;
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
    public class WishlistController : Controller
    {
        private readonly UserIdentityService _userIdentity;
        private readonly UsersService _users;

        public WishlistController(UserIdentityService userIdentity, UsersService users)
        {
            _userIdentity = userIdentity;
            _users = users;
        }

        [Route("/account/wishlist")]
        public async Task<IActionResult> Index()
        {
            User user = await _userIdentity.GetCurrentAsync();
            if (user == null)
                return NotFound();

            List<UserWishlistProduct> wishlist = await _users.GetWishlistAsync(user.Id);

            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View(wishlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return BadRequest();

            await _users.RemoveFromWishlistAsync(userId.Value, productId);
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return BadRequest();

            await _users.AddToWishlistAsync(userId.Value, productId);
            return Json(true);
        }

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> MiniWishlistRemove(int productId)
        {
            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return BadRequest();

            await _users.RemoveFromWishlistAsync(userId.Value, productId);
            return Json(true);
        }

        #endregion

    }
}
