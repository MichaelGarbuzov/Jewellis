using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Services.ClientShoppingCart;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class CartController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly ClientShoppingCartService _clientCart;

        public CartController(JewellisDbContext dbContext, ClientShoppingCartService clientCart)
        {
            _dbContext = dbContext;
            _clientCart = clientCart;
        }

        [Route("/cart")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            await _clientCart.RemoveProductAsync(productId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProceedToCheckout(string cartJson)
        {
            // Performs the quantity updates if needed:
            List<ClientCartProduct> cartProducts = JsonConvert.DeserializeObject<List<ClientCartProduct>>(cartJson);
            for (int i = 0; i < _clientCart.Cart.Products.Count; i++)
            {
                if ((_clientCart.Cart.Products[i].ProductId == cartProducts[i].ProductId) &&
                    (_clientCart.Cart.Products[i].Quantity != cartProducts[i].Quantity))
                {
                    await _clientCart.SetProductAsync(cartProducts[i].ProductId, cartProducts[i].Quantity);
                }
            }
            return RedirectToAction(nameof(OrderController.Checkout), "Order", new { area = "Shop" });
        }

        #region AJAX Actions

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> MiniCartSet(int productId, int? quantity = 1)
        {
            await _clientCart.SetProductAsync(productId, quantity.Value);
            return Json(true);
        }

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> MiniCartRemove(int productId)
        {
            await _clientCart.RemoveProductAsync(productId);
            return Json(true);
        }

        #endregion

    }
}
