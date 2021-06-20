using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Services.ClientShoppingCart;
using Jewellis.Areas.Shop.ViewModels.Order;
using Jewellis.Areas.Shop.ViewModels.Order.Helpers;
using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Jewellis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class OrderController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly UserIdentityService _userIdentity;
        private readonly UsersService _users;
        private readonly AuthenticateService _authenticateService;
        private readonly ClientShoppingCartService _clientCart;

        public OrderController(JewellisDbContext dbContext, UserIdentityService userIdentity, UsersService users, AuthenticateService authenticateService, ClientShoppingCartService clientCart)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
            _users = users;
            _authenticateService = authenticateService;
            _clientCart = clientCart;
        }

        [Route("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            if (_clientCart.IsEmpty())
                return NotFound();

            CheckoutVM model = new CheckoutVM()
            {
                ShippingDetails = new CheckoutVM.ShippingDetailsModel() { ShippingSameAsBilling = true }
            };
            if (_userIdentity.IsAuthenticated())
            {
                // Checks to set already known information about the user in the checkout:
                User user = await _userIdentity.GetCurrentAsync();
                model.BillingDetails = new CheckoutVM.BillingDetailsModel()
                {
                    Name = $"{user.FirstName} {user.LastName}",
                    Phone = user.PhoneNumber
                };
                if (user.Address != null)
                {
                    model.BillingDetails.Street = user.Address.Street;
                    model.BillingDetails.PostalCode = user.Address.PostalCode;
                    model.BillingDetails.City = user.Address.City;
                    model.BillingDetails.Country = user.Address.Country;
                }
            }
            else
            {
                model.SignMethod = SignMethod.Login;
                model.LoginVM = new CheckoutVM.ConditionalLoginVM() { RememberMe = true };
                model.RegisterVM = new CheckoutVM.ConditionalRegisterVM();
            }

            // TODO: for testing:
            //model.BillingDetails = new CheckoutVM.BillingDetailsModel()
            //{
            //    Name = "A B",
            //    Phone = "0541234567",
            //    Street = "ABC",
            //    PostalCode = "12345",
            //    City = "New York",
            //    Country = "USA"
            //};
            //model.CreditCard = new CheckoutVM.CreditCardInfo()
            //{
            //    CardNumber = "123456789",
            //    CardExpiryDate = "04/23",
            //    CardHolderName = "A B",
            //    CardSecurityCode = "444"
            //};
            //model.DeliveryMethodId = 4;

            ViewData["DeliveryMethods"] = await _dbContext.DeliveryMethods.OrderBy(dm => dm.Price).ToListAsync();
            return View(model);
        }

        [Route("/checkout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            if (_clientCart.IsEmpty())
                return NotFound();

            if (ModelState.IsValid)
            {
                // Gets the authenticated user id:
                int? userId = await GetOrAuthenticateUser(model.SignMethod, model.LoginVM, model.RegisterVM);
                if (userId.HasValue)
                {
                    // Binds the order details:
                    Address billingAddress = new Address()
                    {
                        Street = model.BillingDetails.Street,
                        PostalCode = model.BillingDetails.PostalCode,
                        City = model.BillingDetails.City,
                        Country = model.BillingDetails.Country
                    };
                    Address shippingAddress = (model.ShippingDetails == null ? null : new Address()
                    {
                        Street = model.ShippingDetails.Street,
                        PostalCode = model.ShippingDetails.PostalCode,
                        City = model.ShippingDetails.City,
                        Country = model.ShippingDetails.Country
                    });
                    Order order = new Order()
                    {
                        BillingName = model.BillingDetails.Name,
                        BillingPhone = model.BillingDetails.Phone,
                        BillingAddress = billingAddress,
                        ShippingName = (model.ShippingDetails.ShippingSameAsBilling ? model.BillingDetails.Name : model.ShippingDetails.Name),
                        ShippingPhone = (model.ShippingDetails.ShippingSameAsBilling ? model.BillingDetails.Phone : model.ShippingDetails.Phone),
                        ShippingAddress = (model.ShippingDetails.ShippingSameAsBilling ? billingAddress : shippingAddress),
                        DeliveryMethodId = model.DeliveryMethodId.Value,
                        UserId = userId.Value,
                        OrderProducts = new List<OrderVsProduct>(),
                        Note = model.Note,
                        Status = OrderStatus.PaymentProcessing
                    };
                    foreach (var cartProduct in _clientCart.Cart.Products)
                    {
                        OrderVsProduct orderProduct = new OrderVsProduct()
                        {
                            Order = order,
                            ProductId = cartProduct.ProductId,
                            Quantity = cartProduct.Quantity,
                            UnitPrice = cartProduct.Product.Price,
                            DiscountRate = (cartProduct.Product.IsOnSaleNow() ? cartProduct.Product.Sale.DiscountRate : null)
                        };
                        order.OrderProducts.Add(orderProduct);
                    }
                    _dbContext.Orders.Add(order);
                    await _dbContext.SaveChangesAsync();

                    // Clears the shopping cart:
                    await _clientCart.Clear();

                    // Here is a good place to charge the credit card... (but sadly we don't support it)

                    return RedirectToAction(nameof(CheckoutConfirmed), new { orderId = order.Id });
                }
            }

            ViewData["DeliveryMethods"] = await _dbContext.DeliveryMethods.OrderBy(dm => dm.Price).ToListAsync();
            return View(model);
        }

        [Route("/checkout-confirmed")]
        public IActionResult CheckoutConfirmed(int orderId)
        {
            return View(orderId);
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckEmailAvailability(CheckoutVM.ConditionalRegisterVM registerVM)
        {
            bool isEmailAvailable = await _users.IsEmailAddressAvailable(registerVM.EmailAddress);
            return Json(isEmailAvailable);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the id of the current authenticated user, or logins/registers the current user by the specified parameters.
        /// </summary>
        /// <remarks>This modifies the ViewData also, if there are errors.</remarks>
        /// <param name="signMethod">The sign method to use: login/register.</param>
        /// <param name="loginVM">The login view model of the checkout view model.</param>
        /// <param name="registerVM">The register view model of the checkout view model.</param>
        /// <returns>Returns the user id if authentication succeeded, otherwise null.</returns>
        private async Task<int?> GetOrAuthenticateUser(SignMethod? signMethod, CheckoutVM.ConditionalLoginVM loginVM, CheckoutVM.ConditionalRegisterVM registerVM)
        {
            // Checks if the user is already authenticated:
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return HttpContext.User.Identity.GetId().Value;
            }
            // Otherwise, needs to authenticate the user:
            else
            {
                User user;
                if (signMethod == SignMethod.Login)
                {
                    user = await _authenticateService.LoginAsync(loginVM.EmailAddress, loginVM.Password, loginVM.RememberMe);
                    if (user == null)
                    {
                        ViewData["Error_Login"] = "Incorrect email address or password.";
                    }
                }
                else if (signMethod == SignMethod.Register)
                {
                    user = await _authenticateService.RegisterAsync(registerVM.FirstName, registerVM.LastName, registerVM.EmailAddress, registerVM.Password, registerVM.SubscribeNewsletter);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(signMethod), $"{nameof(signMethod)} is not defined.");
                }

                if (user != null)
                    return user.Id;
                else
                    return null;
            }
        }

        #endregion

    }
}
