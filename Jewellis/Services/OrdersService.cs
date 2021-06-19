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
    /// Represents a service for interacting with orders data.
    /// </summary>
    public class OrdersService
    {
        private readonly JewellisDbContext _dbContext;

        public OrdersService(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Public API

        /// <summary>
        /// Gets the order by the id.
        /// </summary>
        /// <param name="orderId">The id of the order to get.</param>
        /// <returns>Returns the order info if found, otherwise null.</returns>
        public async Task<Order> GetByIdAsync(int orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.BillingAddress).Include(o => o.ShippingAddress).Include(o => o.DeliveryMethod).Include(o => o.User)
                .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        /// <summary>
        /// Gets all the orders created from the specified date and time.
        /// </summary>
        /// <param name="fromDateTime">The date and time to select the orders created after.</param>
        /// <returns>Returns the list of orders created from the specified date and time.</returns>
        public async Task<List<Order>> GetAllFromDateTime(DateTime fromDateTime)
        {
            return await _dbContext.Orders
                .Where(o => o.DateCreated >= fromDateTime)
                .Include(o => o.DeliveryMethod)
                .OrderByDescending(o => o.DateCreated)
                .Include(o => o.OrderProducts)
                .ToListAsync();
        }

        /// <summary>
        /// Searches for orders by the specified parameters.
        /// </summary>
        /// <param name="status">The status of orders to search.</param>
        /// <param name="dateCreated">The date the orders created to search.</param>
        /// <param name="orderId">The id of the order to search.</param>
        /// <returns>Returns the list of orders matching the specified search parameters.</returns>
        public async Task<List<Order>> Search(OrderStatus? status, DateTime? dateCreated, string orderId)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                orderId = orderId.TrimStart('#');
            }

            return await _dbContext.Orders
                .Where(o => ((orderId == null) || o.Id.ToString().Equals(orderId)) &&
                            ((status == null) || o.Status == status.Value) &&
                            ((dateCreated == null) || o.DateCreated.Date == dateCreated.Value.Date))
                .Include(o => o.DeliveryMethod)
                .OrderByDescending(o => o.DateCreated)
                .Include(o => o.OrderProducts)
                .ToListAsync();
        }

        /// <summary>
        /// Deletes the specified order.
        /// </summary>
        /// <param name="orderId">The id of the order to delete.</param>
        /// <returns>Returns true if the delete has succeeded, otherwise false.</returns>
        public async Task<bool> DeleteOrder(int orderId)
        {
            Order order = await this.GetByIdAsync(orderId);
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates the status of the specified order.
        /// </summary>
        /// <param name="orderId">The id of the order to update.</param>
        /// <param name="status">The status of the order.</param>
        /// <returns>Returns true if the update has succeeded, otherwise false.</returns>
        public async Task<bool> UpdateStatus(int orderId, OrderStatus status)
        {
            Order order = await this.GetByIdAsync(orderId);

            order.Status = status;

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

    }
}
