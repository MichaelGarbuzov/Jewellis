using Jewellis.App_Custom.Helpers;
using Jewellis.App_Custom.Services.AuthUser;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.App_Custom.Services.ClientShoppingCart
{
    /// <summary>
    /// Represents a service (scoped) for managing the client's shopping cart.
    /// </summary>
    public class ClientShoppingCartService
    {
        #region Constants

        private const string CART_COOKIE = AppKeys.Cookies.ClientCart;
        private const string CACHE_IDENTIFIER = AppKeys.Cache.ClientCart;
        private const string ENCRYPTION_KEY = "f5%D4@#14dDdf$%S5d4$p@1Fc";

        #endregion

        #region Private Members

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly JewellisDbContext _dbContext;
        private readonly AuthUserService _authUser;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current client's cart - in this scope.
        /// </summary>
        public ClientCart Cart { get; private set; }

        #endregion

        /// <summary>
        /// Represents a service (scoped) for managing the client's shopping cart.
        /// </summary>
        public ClientShoppingCartService(IHttpContextAccessor httpContextAccessor, IMemoryCache cache, JewellisDbContext dbContext, AuthUserService authUser)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _dbContext = dbContext;
            _authUser = authUser;

            this.InitializeClientCart();
        }

        #region Public API

        /// <summary>
        /// Checks whether the client's cart is empty or not.
        /// </summary>
        /// <returns>Returns true if the client's cart is empty, otherwise false.</returns>
        public bool IsEmpty()
        {
            return (Cart == null || Cart.Products == null || Cart.Products.Count < 1);
        }

        /// <summary>
        /// Gets the number of products in the cart.
        /// </summary>
        /// <returns>Returns the number of products in the cart.</returns>
        public int Count()
        {
            return (Cart == null || Cart.Products == null ? 0 : Cart.Products.Count);
        }

        /// <summary>
        /// Gets the subtotal price of products (before discount calculation) in the shopping cart.
        /// </summary>
        /// <returns>Returns the subtotal price of products (before discount calculation) in the shopping cart.</returns>
        public double GetSubtotal()
        {
            double subtotal = 0;
            if (!this.IsEmpty())
            {
                foreach (var product in Cart.Products)
                {
                    subtotal += (product.Product.Price * product.Quantity);
                }
            }
            return subtotal;
        }

        /// <summary>
        /// Gets the total discount of products in the shopping cart.
        /// </summary>
        /// <returns>Returns the total discount of products in the shopping cart.</returns>
        public double GetTotalDiscount()
        {
            double totalDiscount = 0;
            if (!this.IsEmpty())
            {
                foreach (var product in Cart.Products)
                {
                    totalDiscount += ((product.Product.Price - product.Product.ActualPrice()) * product.Quantity);
                }
            }
            return totalDiscount;
        }

        /// <summary>
        /// Gets the total price of products (after discount calculation) in the shopping cart.
        /// </summary>
        /// <returns>Returns the total price of products (after discount calculation) in the shopping cart.</returns>
        public double GetTotalPrice()
        {
            double totalPrice = 0;
            if (!this.IsEmpty())
            {
                foreach (var product in Cart.Products)
                {
                    totalPrice += (product.Product.ActualPrice() * product.Quantity);
                }
            }
            return totalPrice;
        }

        /// <summary>
        /// Gets the public identifier for the client's cart id.
        /// </summary>
        /// <returns>Returns the public identifier for the client's cart id if cart is found, otherwise null.</returns>
        public string GetClientCartIdPublic()
        {
            if (!this.IsEmpty())
            {
                return EncryptionHelper.EncryptAES(this.Cart.Id.ToString(), ENCRYPTION_KEY);
            }
            return null;
        }

        /// <summary>
        /// Sets (adds or updates) the specified product and quantity in the cart.
        /// </summary>
        /// <param name="productId">The id of the product to set.</param>
        /// <param name="quantity">The quantity to set.</param>
        public async Task SetProductAsync(int productId, int quantity)
        {
            if (quantity < 1)
                throw new ArgumentException($"{nameof(quantity)} cannot be less than 1.", nameof(quantity));

            // First, checks the product exists and valid in the database:
            if (!await _dbContext.Products.AnyAsync(p => p.Id == productId && p.IsAvailable))
                return;

            // Creates/Updates the cart:
            // Now, checks if it's the first item in the cart:
            if (this.IsEmpty())
            {
                // So creates a new cart:
                this.Cart = new ClientCart();
                this.Cart.Products = new List<ClientCartProduct>();
                ClientCartProduct cartProduct = new ClientCartProduct()
                {
                    ClientCart = Cart,
                    ProductId = productId,
                    Quantity = quantity
                };
                this.Cart.Products.Add(cartProduct);
                _dbContext.ClientCarts.Add(this.Cart);
            }
            // Otherwise, it's not the first item:
            else
            {
                // Checks if the product already exists in the cart:
                ClientCartProduct cartProduct = this.Cart.Products.FirstOrDefault(p => p.ProductId == productId);
                if (cartProduct != null)
                {
                    // Updates the item:
                    cartProduct.Quantity = quantity;
                    _dbContext.ClientCartProducts.Update(cartProduct);
                }
                else
                {
                    // Adds the item:
                    cartProduct = new ClientCartProduct()
                    {
                        ClientCartId = this.Cart.Id,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    _dbContext.ClientCartProducts.Add(cartProduct);
                }
            }

            // If the user is authenticated - connects the cart to him:
            User user = await _authUser.GetAsync();
            if (user != null && user.ClientCartId == null)
            {
                user.ClientCart = this.Cart;
                _dbContext.Users.Update(user);
            }

            await _dbContext.SaveChangesAsync();

            // Gets the cart back from the database:
            ClientCart clientCartBack = null;
            if (user != null)
            {
                clientCartBack = Task.Run(() => GetAuthUserCartByDatabase()).Result;
                user.ClientCartId = clientCartBack.Id;
                _authUser.Set(user);
                this.CacheAuthUserCart(user.Id, clientCartBack);
            }
            else
            {
                clientCartBack = Task.Run(() => GetClientCartByDatabase(this.Cart.Id)).Result;
                this.CacheClientCart(clientCartBack);
            }

            // If user is anonymous - sets his cart id to the cookie:
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == false)
            {
                this.SetUserCartIdToCookie(this.Cart.Id);
            }
        }

        /// <summary>
        /// Removes the specified product from the cart.
        /// </summary>
        /// <param name="productId">The id of the product to remove.</param>
        public async Task RemoveProductAsync(int productId)
        {
            // First, checks there are items in the cart:
            if (this.IsEmpty())
                return;

            ClientCartProduct cartProduct = this.Cart.Products.FirstOrDefault(i => i.ProductId == productId);
            if (cartProduct != null)
            {
                _dbContext.ClientCartProducts.Remove(cartProduct);
                this.Cart.Products.Remove(cartProduct);
                if (this.Cart.Products.Count < 1)
                {
                    _dbContext.ClientCarts.Remove(this.Cart);
                }
                await _dbContext.SaveChangesAsync();
            }

            // If empty - deletes the cart id cookie and removes from the cache:
            if (this.IsEmpty())
            {
                if (_authUser.IsAuthenticated())
                {
                    User user = await _authUser.GetAsync();
                    user.ClientCartId = null;
                    user.ClientCart = null;
                    _authUser.Set(user);
                    this.RemoveCacheAuthUserCart(user.Id);
                }
                else
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete(CART_COOKIE);
                    this.RemoveCacheClientCart(this.Cart.Id);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the client's cart, either from the database (if user is authenticated) or the cookie.
        /// </summary>
        /// <remarks>Using memory cache for performance improvments.</remarks>
        private void InitializeClientCart()
        {
            ClientCart clientCart = null;

            // (1) - Checks if the user is authenticated - then gets the cart from the DB:
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // First checks if the cart is in the cache memory:
                int? userId = _httpContextAccessor.HttpContext.User.Identity.GetId();
                ClientCart userCart;
                if (_cache.TryGetValue($"{CACHE_IDENTIFIER}_user_{userId.Value}", out userCart))
                {
                    clientCart = userCart;
                }
                else
                {
                    clientCart = Task.Run(() => GetAuthUserCartByDatabase()).Result;
                    // Inserts the user's cart into cache:
                    if (clientCart != null)
                        this.CacheAuthUserCart(userId.Value, clientCart);
                }
            }

            // (2) - User is not authenticated - so gets the cart from the cookie:
            if (clientCart == null)
            {
                // First checks if the cart is in the cache memory:
                int? cartId = this.GetUserCartIdByCookie();
                if (cartId.HasValue)
                {
                    ClientCart cCart;
                    if (_cache.TryGetValue($"{CACHE_IDENTIFIER}_{cartId.Value}", out cCart))
                    {
                        clientCart = cCart;
                    }
                    else
                    {
                        clientCart = Task.Run(() => GetClientCartByDatabase(cartId.Value)).Result;
                        // Inserts the client's cart into cache:
                        if (clientCart != null)
                            this.CacheClientCart(clientCart);
                    }
                }
            }

            // Assigns the cart found to the current scope:
            this.Cart = clientCart;
        }

        /// <summary>
        /// Gets the shopping cart of the user by the database (for an authenticated user).
        /// </summary>
        /// <returns>Returns the shopping cart of the user by the database if authenticated, otherwise null.</returns>
        private async Task<ClientCart> GetAuthUserCartByDatabase()
        {
            int? userId = _httpContextAccessor.HttpContext.User.Identity.GetId();
            if (userId != null)
            {
                var userCart = await (from user in _dbContext.Users
                                      where (user.Id == userId.Value)
                                      join cart in _dbContext.ClientCarts on user.ClientCartId equals cart.Id
                                      join cartProduct in _dbContext.ClientCartProducts on cart.Id equals cartProduct.ClientCartId
                                      join product in _dbContext.Products on cartProduct.ProductId equals product.Id
                                      join sale in _dbContext.Sales on product.SaleId equals sale.Id into sj
                                      from saleJoin in sj.DefaultIfEmpty()
                                      orderby cartProduct.DateAdded descending
                                      select new
                                      {
                                          Cart = cart,
                                          CartProduct = cartProduct,
                                          Product = product,
                                          Sale = saleJoin
                                      }).ToListAsync();

                if (userCart != null && userCart.Count > 0)
                    return userCart[0].Cart;
            }
            return null;
        }

        /// <summary>
        /// Gets the shopping cart of the client by the database.
        /// </summary>
        /// <returns>Returns the shopping cart of the client by the database if found, otherwise null.</returns>
        private async Task<ClientCart> GetClientCartByDatabase(int cartId)
        {
            var clientCart = await (from cart in _dbContext.ClientCarts
                                    where (cart.Id == cartId)
                                    join cartProduct in _dbContext.ClientCartProducts on cart.Id equals cartProduct.ClientCartId
                                    join product in _dbContext.Products on cartProduct.ProductId equals product.Id
                                    join sale in _dbContext.Sales on product.SaleId equals sale.Id into sj
                                    from saleJoin in sj.DefaultIfEmpty()
                                    orderby cartProduct.DateAdded descending
                                    select new
                                    {
                                        Cart = cart,
                                        CartProduct = cartProduct,
                                        Product = product,
                                        Sale = saleJoin
                                    }).ToListAsync();

            if (clientCart != null && clientCart.Count > 0)
                return clientCart[0].Cart;
            else
                return null;
        }

        /// <summary>
        /// Gets the id of client's shopping cart by the cookie.
        /// </summary>
        /// <returns>Returns the id of client's shopping cart by the cookie if found, otherwise null.</returns>
        private int? GetUserCartIdByCookie()
        {
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[CART_COOKIE];
            if (!string.IsNullOrEmpty(cookieValue))
            {
                // Decrypts the cart id:
                string decryptedValue = EncryptionHelper.DecryptAES(cookieValue, ENCRYPTION_KEY);
                int cartId;
                if (int.TryParse(decryptedValue, out cartId))
                {
                    return cartId;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the id of client's shopping cart to the cookie.
        /// </summary>
        /// <param name="cartId">The id of the client's cart in the database.</param>
        /// <remarks>NOTE: Safe, since this performs encryption to the database id before setting it to the cookie.</remarks>
        private void SetUserCartIdToCookie(int clientCartId)
        {
            // Encrypts the cart id:
            string encryptedValue = EncryptionHelper.EncryptAES(clientCartId.ToString(), ENCRYPTION_KEY);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CART_COOKIE, encryptedValue, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddMonths(1)
            });
        }

        /// <summary>
        /// Inserts the specified authenticated user's cart into the cache memory.
        /// </summary>
        /// <param name="userId">The id of the authenticated user.</param>
        /// <param name="userCart">The authenticated user's cart to cache.</param>
        private void CacheAuthUserCart(int userId, ClientCart userCart)
        {
            if (userCart == null)
                throw new ArgumentNullException(nameof(userCart), $"{nameof(userCart)} cannot be null.");

            _cache.Set($"{CACHE_IDENTIFIER}_user_{userId}", userCart, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }

        /// <summary>
        /// Inserts the specified client's cart into the cache memory.
        /// </summary>
        /// <param name="clientCart">The client's cart to cache.</param>
        private void CacheClientCart(ClientCart clientCart)
        {
            if (clientCart == null)
                throw new ArgumentNullException(nameof(clientCart), $"{nameof(clientCart)} cannot be null.");

            _cache.Set($"{CACHE_IDENTIFIER}_{clientCart.Id}", clientCart, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }

        /// <summary>
        /// Removes the specified authenticated user's cart from the cache memory.
        /// </summary>
        /// <param name="userCart">The authenticated user's cart to remove from cache.</param>
        private void RemoveCacheAuthUserCart(int userId)
        {
            _cache.Remove($"{CACHE_IDENTIFIER}_user_{userId}");
        }

        /// <summary>
        /// Removes the specified client's cart from the cache memory.
        /// </summary>
        /// <param name="clientCart">The client's cart to remove from cache.</param>
        private void RemoveCacheClientCart(int clientCartId)
        {
            _cache.Remove($"{CACHE_IDENTIFIER}_{clientCartId}");
        }

        #endregion

    }
}
