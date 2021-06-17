using Jewellis.App_Custom.Helpers;
using Jewellis.App_Custom.Services.UserCache;
using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Services
{
    /// <summary>
    /// Represents a service for interacting with authenticated users data.
    /// </summary>
    public class UsersService
    {
        private readonly JewellisDbContext _dbContext;
        private readonly UserCacheService _userCache;

        public UsersService(JewellisDbContext dbContext, UserCacheService userCache)
        {
            _dbContext = dbContext;
            _userCache = userCache;
        }

        #region Public API

        /// <summary>
        /// Gets the user by the id, either from the cache or the database.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <returns>Returns the user info, either from the cache or the database if found, otherwise null.</returns>
        public async Task<User> GetByIdAsync(int userId)
        {
            // Gets the user from cache:
            User user = _userCache.Get(userId);
            if (user == null)
            {
                // User not found in cache - so gets from the database:
                user = await GetUserByIdFromDatabase(userId);
                if (user == null)
                    throw new Exception("The user id was not found in the database.");

                _userCache.Set(user);
            }
            return user;
        }

        /// <summary>
        /// Gets the user from the database by the email address.
        /// </summary>
        /// <param name="emailAddress">The email address of the user to get.</param>
        /// <returns>Returns the user from the database if found, otherwise null.</returns>
        public async Task<User> GetUserByEmailAsync(string emailAddress)
        {
            var query = await (from user in _dbContext.Users.Include(u => u.Address)
                               where (user.EmailAddress == emailAddress)
                               join uwp in _dbContext.UserWishlistProducts on user.Id equals uwp.UserId into uwps
                               from uwp in uwps.DefaultIfEmpty()
                               join product in _dbContext.Products on uwp.ProductId equals product.Id into ps
                               from product in ps.DefaultIfEmpty()
                               join sale in _dbContext.Sales on product.SaleId equals sale.Id into sj
                               from sale in sj.DefaultIfEmpty()
                               select new
                               {
                                   User = user,
                                   Uwp = uwp,
                                   Product = product,
                                   Sale = sale
                               }).ToListAsync();

            if (query != null && query.Count > 0)
            {
                User user = query[0].User;
                if (user.Wishlist == null)
                    user.Wishlist = new List<UserWishlistProduct>();
                return user;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the profile info of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <param name="phoneNumber">The phone number of the user.</param>
        /// <returns>Returns true if the update has succeeded, otherwise false.</returns>
        public async Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, string emailAddress, string phoneNumber)
        {
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException(nameof(firstName), $"{nameof(firstName)} cannot be null or empty.");
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentNullException(nameof(lastName), $"{nameof(lastName)} cannot be null or empty.");
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress), $"{nameof(emailAddress)} cannot be null or empty.");

            User user = await this.GetByIdAsync(userId);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.EmailAddress = emailAddress;
            user.PhoneNumber = phoneNumber;
            user.DateLastModified = DateTime.Now;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            _userCache.Set(user);
            return true;
        }

        /// <summary>
        /// Updates the password of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password of the user.</param>
        /// <returns>Returns true if succeeded, otherwise false (meaning the current password is incorrect).</returns>
        public async Task<bool> UpdateUserPassword(int userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword))
                throw new ArgumentNullException(nameof(currentPassword), $"{nameof(currentPassword)} cannot be null or empty.");
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentNullException(nameof(newPassword), $"{nameof(newPassword)} cannot be null or empty.");

            User user = await this.GetByIdAsync(userId);

            // First, checks the current password is correct:
            string currentPasswordHash = EncryptionHelper.HashSHA256(currentPassword + user.PasswordSalt);
            if (await _dbContext.Users.AnyAsync(u => u.Id == user.Id && u.PasswordHash.Equals(currentPasswordHash)))
            {
                // Binds the view model:
                user.PasswordSalt = EncryptionHelper.GenerateSalt();
                user.PasswordHash = EncryptionHelper.HashSHA256(newPassword + user.PasswordSalt);
                user.DateLastModified = DateTime.Now;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                _userCache.Set(user);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the preferences of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="currency">The preferred currency code of the user.</param>
        /// <param name="theme">The preferred theme id of the user.</param>
        /// <returns>Returns true if the update has succeeded, otherwise false.</returns>
        public async Task<bool> UpdateUserPreferences(int userId, string currency, string theme)
        {
            if (string.IsNullOrEmpty(currency))
                throw new ArgumentNullException(nameof(currency), $"{nameof(currency)} cannot be null or empty.");
            if (string.IsNullOrEmpty(theme))
                throw new ArgumentNullException(nameof(theme), $"{nameof(theme)} cannot be null or empty.");

            User user = await this.GetByIdAsync(userId);

            user.Currency = currency;
            user.Theme = theme;
            user.DateLastModified = DateTime.Now;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            _userCache.Set(user);
            return true;
        }

        /// <summary>
        /// Updates the address of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="street">The address street of the user.</param>
        /// <param name="postalCode">The address postal code of the user.</param>
        /// <param name="city">The address city of the user.</param>
        /// <param name="country">The address country of the user.</param>
        /// <returns>Returns true if the update has succeeded, otherwise false.</returns>
        public async Task<bool> UpdateUserAddress(int userId, string street, string postalCode, string city, string country)
        {
            if (string.IsNullOrEmpty(street))
                throw new ArgumentNullException(nameof(street), $"{nameof(street)} cannot be null or empty.");
            if (string.IsNullOrEmpty(postalCode))
                throw new ArgumentNullException(nameof(postalCode), $"{nameof(postalCode)} cannot be null or empty.");
            if (string.IsNullOrEmpty(city))
                throw new ArgumentNullException(nameof(city), $"{nameof(city)} cannot be null or empty.");
            if (string.IsNullOrEmpty(country))
                throw new ArgumentNullException(nameof(country), $"{nameof(country)} cannot be null or empty.");

            User user = await this.GetByIdAsync(userId);

            // Checks if the user has no address yet:
            if (user.Address == null)
                user.Address = new Address();

            user.Address.Street = street;
            user.Address.PostalCode = postalCode;
            user.Address.City = city;
            user.Address.Country = country;
            user.Address.DateLastModified = DateTime.Now;
            user.DateLastModified = DateTime.Now;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            _userCache.Set(user);
            return true;
        }

        /// <summary>
        /// Removes the address of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="street">The address street of the user.</param>
        /// <param name="postalCode">The address postal code of the user.</param>
        /// <param name="city">The address city of the user.</param>
        /// <param name="country">The address country of the user.</param>
        /// <returns>Returns true if the update has succeeded, otherwise false.</returns>
        public async Task<bool> RemoveUserAddress(int userId)
        {
            User user = await this.GetByIdAsync(userId);
            if (user.Address != null)
            {
                _dbContext.Addresses.Remove(user.Address);
                await _dbContext.SaveChangesAsync();

                user.AddressId = null;
                user.Address = null;
                _userCache.Set(user);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether or not an email address is available (no user is using it).
        /// </summary>
        /// <param name="emailAddress">The email address to check.</param>
        /// <returns>Returns true if the email address is available (no user is using it), otherwise false.</returns>
        public async Task<bool> IsEmailAddressAvailable(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress), $"{nameof(emailAddress)} cannot be null or empty.");

            return (await _dbContext.Users.AnyAsync(u => u.EmailAddress.Equals(emailAddress)) == false);
        }

        /// <summary>
        /// Edits the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <returns>Returns true if the update has succeeded, otherwise false.</returns>
        public async Task<bool> EditUser(int userId, string firstName, string lastName, string emailAddress, UserRole role)
        {
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException(nameof(firstName), $"{nameof(firstName)} cannot be null or empty.");
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentNullException(nameof(lastName), $"{nameof(lastName)} cannot be null or empty.");
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress), $"{nameof(emailAddress)} cannot be null or empty.");

            User user = await this.GetByIdAsync(userId);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.EmailAddress = emailAddress;
            user.Role = role;
            user.DateLastModified = DateTime.Now;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            _userCache.Set(user);
            return true;
        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to delete.</param>
        /// <returns>Returns true if the delete has succeeded, otherwise false.</returns>
        public async Task<bool> DeleteUser(int userId)
        {
            User user = await this.GetByIdAsync(userId);
            if (user.ClientCartId.HasValue)
            {
                ClientCart cart = await _dbContext.ClientCarts.FindAsync(user.ClientCartId.Value);
                _dbContext.ClientCarts.Remove(cart);
            }
            _dbContext.Addresses.Remove(user.Address);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            _userCache.Remove(user.Id);
            return true;
        }

        /// <summary>
        /// Gets the wishlist of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to get the wishlist.</param>
        /// <returns>Returns the wishlist of the specified user.</returns>
        public async Task<List<UserWishlistProduct>> GetWishlistAsync(int userId)
        {
            List<UserWishlistProduct> wishlist = await _dbContext.UserWishlistProducts
                .Where(uwp => uwp.UserId == userId)
                .Include(uwp => uwp.Product).Include(uwp => uwp.Product.Sale)
                .OrderByDescending(uwp => uwp.DateAdded)
                .ToListAsync();

            return wishlist;
        }

        /// <summary>
        /// Adds the product to the wishlist of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to add to.</param>
        /// <param name="productId">The id of the product to add.</param>
        public async Task AddToWishlistAsync(int userId, int productId)
        {
            // Checks the product is not in the user's wishlist already:
            if (await _dbContext.UserWishlistProducts.AnyAsync(uwp => uwp.UserId == userId && uwp.ProductId == productId) == false)
            {
                // Adds to the database:
                UserWishlistProduct wishlistProduct = new UserWishlistProduct()
                {
                    UserId = userId,
                    ProductId = productId
                };
                _dbContext.UserWishlistProducts.Add(wishlistProduct);
                await _dbContext.SaveChangesAsync();

                // Adds to the cache:
                wishlistProduct = await _dbContext.UserWishlistProducts
                    .Include(uwp => uwp.Product).Include(uwp => uwp.Product.Sale)
                    .FirstOrDefaultAsync(uwp => uwp.UserId == userId && uwp.ProductId == productId);

                User user = await this.GetByIdAsync(userId);
                user.Wishlist.Add(wishlistProduct);
                _userCache.Set(user);
            }
        }

        /// <summary>
        /// Removes the product from the wishlist of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to remove from.</param>
        /// <param name="productId">The id of the product to remove.</param>
        public async Task RemoveFromWishlistAsync(int userId, int productId)
        {
            User user = await this.GetByIdAsync(userId);

            // Checks the product is in the user's wishlist:
            UserWishlistProduct wishlistProduct = user.Wishlist.FirstOrDefault(uwp => uwp.ProductId == productId);
            if (wishlistProduct != null)
            {
                // Updates the database:
                _dbContext.UserWishlistProducts.Remove(wishlistProduct);
                await _dbContext.SaveChangesAsync();
                // Updates the cache:
                user.Wishlist.Remove(wishlistProduct);
                _userCache.Set(user);
            }
        }

        /// <summary>
        /// Gets the orders of the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to get the orders.</param>
        /// <returns>Returns the orders of the specified user.</returns>
        public async Task<List<Order>> GetOrdersAsync(int userId)
        {
            List<Order> orders = await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.DeliveryMethod).Include(o => o.OrderProducts)
                .OrderByDescending(o => o.DateCreated)
                .ToListAsync();

            return orders;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the user info from the database.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <returns>Returns the user info from the database if found, otherwise null.</returns>
        private async Task<User> GetUserByIdFromDatabase(int userId)
        {
            var query = await (from user in _dbContext.Users.Include(u => u.Address)
                               where (user.Id == userId)
                               join uwp in _dbContext.UserWishlistProducts on user.Id equals uwp.UserId into uwps
                               from uwp in uwps.DefaultIfEmpty()
                               join product in _dbContext.Products on uwp.ProductId equals product.Id into ps
                               from product in ps.DefaultIfEmpty()
                               join sale in _dbContext.Sales on product.SaleId equals sale.Id into sj
                               from sale in sj.DefaultIfEmpty()
                               select new
                               {
                                   User = user,
                                   Uwp = uwp,
                                   Product = product,
                                   Sale = sale
                               }).ToListAsync();

            if (query != null && query.Count > 0)
            {
                User user = query[0].User;
                if (user.Wishlist == null)
                    user.Wishlist = new List<UserWishlistProduct>();
                return user;
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}
