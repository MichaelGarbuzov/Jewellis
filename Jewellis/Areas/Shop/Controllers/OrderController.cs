using Jewellis.App_Custom.Services.ClientShoppingCart;
using Jewellis.Data;
using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class OrderController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly ClientShoppingCartService _clientCart;

        public OrderController(JewellisDbContext dbContext, ClientShoppingCartService clientCart)
        {
            _dbContext = dbContext;
            _clientCart = clientCart;
        }

        [Route("/checkout")]
        public IActionResult Checkout()
        {
            if (_clientCart.IsEmpty())
                return NotFound();



            return View();
        }

        [Route("/checkout-confirmed")]
        public IActionResult CheckoutConfirmed()
        {
            return View();
        }

    }
}
