using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Services.AuthUser;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly AuthUserService _authUser;

        public WishlistController(JewellisDbContext dbContext, AuthUserService authUser)
        {
            _dbContext = dbContext;
            _authUser = authUser;
        }

        [Route("/account/wishlist")]
        public async Task<IActionResult> Index()
        {
            User user = await _authUser.GetAsync();
            if (user == null)
                return NotFound();

            List<UserWishlistProduct> wishlist = await _dbContext.UserWishlistProducts
                .Where(uwp => uwp.UserId == user.Id)
                .Include(uwp => uwp.Product).Include(uwp => uwp.Product.Sale)
                .OrderByDescending(uwp => uwp.DateAdded)
                .ToListAsync();

            ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            return View(wishlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            User user = await _authUser.GetAsync();
            if (user == null)
                return BadRequest();

            await this.RemoveProductFromUserWishlist(user, productId);
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            User user = await _authUser.GetAsync();
            if (user == null)
                return BadRequest();

            // Checks the product is not in the user's wishlist already:
            if (await _dbContext.UserWishlistProducts.AnyAsync(uwp => uwp.UserId == user.Id && uwp.ProductId == productId) == false)
            {
                // Adds to the database:
                UserWishlistProduct wishlistProduct = new UserWishlistProduct()
                {
                    UserId = user.Id,
                    ProductId = productId
                };
                _dbContext.UserWishlistProducts.Add(wishlistProduct);
                await _dbContext.SaveChangesAsync();

                // Adds to the cache:
                wishlistProduct = await _dbContext.UserWishlistProducts
                    .Include(uwp => uwp.Product).Include(uwp => uwp.Product.Sale)
                    .FirstOrDefaultAsync(uwp => uwp.UserId == user.Id && uwp.ProductId == productId);
                user.Wishlist.Add(wishlistProduct);
                _authUser.Set(user);
            }

            return Json(true);
        }

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> MiniWishlistRemove(int productId)
        {
            User user = await _authUser.GetAsync();
            if (user == null)
                return BadRequest();

            await this.RemoveProductFromUserWishlist(user, productId);
            return Json(true);
        }

        #endregion

        #region Private Methods

        private async Task RemoveProductFromUserWishlist(User user, int productId)
        {
            // Checks the product is in the user's wishlist:
            UserWishlistProduct wishlistProduct = user.Wishlist.FirstOrDefault(uwp => uwp.ProductId == productId);
            if (wishlistProduct != null)
            {
                // Updates the database:
                _dbContext.UserWishlistProducts.Remove(wishlistProduct);
                await _dbContext.SaveChangesAsync();
                // Updates the cache:
                user.Wishlist.Remove(wishlistProduct);
                _authUser.Set(user);
            }
        }

        #endregion

    }
}
