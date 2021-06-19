using Jewellis.App_Custom.Helpers.Objects;
using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.Areas.Admin.ViewModels.Home;
using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Jewellis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly OrdersService _orders;
        private readonly ClientCurrencyService _clientCurrency;

        public HomeController(JewellisDbContext dbContext, OrdersService orders, ClientCurrencyService clientCurrency)
        {
            _dbContext = dbContext;
            _orders = orders;
            _clientCurrency = clientCurrency;
        }

        public async Task<IActionResult> Index(IndexVM model)
        {
            // Gets the number of orders awaiting:
            ViewData["Results_OrdersAwaiting"] = await _dbContext.Orders.Where(o => o.Status == OrderStatus.Packing).CountAsync();

            // Gets the number of pending contacts:
            ViewData["Results_PendingContacts"] = await _dbContext.Contacts.Where(c => c.Status == ContactStatus.Pending).CountAsync();

            #region Data here is related to the specified period...

            DateTime fromDate = this.GetDateTimeStartOfPeriod(model.Period);

            // Gets the number of new customers (in this period):
            ViewData["Results_NewCustomers"] = await _dbContext.Users.Where(u => u.DateRegistered >= fromDate && u.Role == UserRole.Customer).CountAsync();

            // Gets the best selling products (how many times a product was ordered):
            var bestSellingQuery = await (from order in _dbContext.Orders
                                          where order.DateCreated >= fromDate
                                          join op in _dbContext.OrdersVsProducts on order.Id equals op.OrderId
                                          join product in _dbContext.Products on op.ProductId equals product.Id
                                          group order.Id by product.Name into grp
                                          orderby grp.Count() descending
                                          select new
                                          {
                                              ProductName = grp.Key,
                                              OrderCount = grp.Count().ToString()
                                          }).Take(10).ToListAsync();
            ViewData["Results_BestSelling"] = bestSellingQuery.ToDictionary(d => d.ProductName, d => d.OrderCount);
            ViewData["Results_BestSelling_Top3"] = bestSellingQuery.Take(3).Select(d => d.ProductName).ToArray();

            // Gets all the orders related to the specified period:
            List<Order> orders = await _orders.GetAllFromDateTime(fromDate);

            // If period is liftime - overrides the fromDate to be the first record:
            if (orders.Count > 0 && model.Period == Periods.Lifetime)
                fromDate = orders.Last().DateCreated;

            // Gets the total earnings and earnings time series chart:
            double totalEarnings = 0;
            TimeSeriesChartData earningsTS = new TimeSeriesChartData(model.Period, fromDate);
            foreach (Order order in orders)
            {
                double orderTotal = order.GetTotalPrice();
                totalEarnings += orderTotal;
                earningsTS.Add(order.DateCreated, _clientCurrency.GetPrice(orderTotal));
            }
            ViewData["Results_TotalEarnings"] = _clientCurrency.GetPrice(totalEarnings);
            ViewData["Results_EarningsTimeSeries"] = earningsTS;

            // Gets the average customer spending time series chart:
            TimeSeriesChartData avgCustomerSpendingTS = new TimeSeriesChartData(model.Period, fromDate);
            foreach (Order order in orders)
            {
                avgCustomerSpendingTS.Add(order.DateCreated, _clientCurrency.GetPrice(order.GetTotalPrice()));
            }
            ViewData["Results_AvgCustomerSpendingTS"] = avgCustomerSpendingTS;

            // Gets the average customer spending time series chart:
            TimeSeriesChartData avgCustomerActivityTimesTS = new TimeSeriesChartData(model.Period, fromDate);
            foreach (Order order in orders)
            {
                avgCustomerActivityTimesTS.Add(order.DateCreated, order.DateCreated.RoundUp(TimeSpan.FromMinutes(30)).Hour);
            }
            ViewData["Results_AvgCustomerActivityTimesTS"] = avgCustomerActivityTimesTS;

            #endregion

            return View(model);
        }

        #region Private Methods

        /// <summary>
        /// Gets the date and time of the period start.
        /// </summary>
        /// <param name="period">The period to get the start date time.</param>
        /// <returns>Returns the date and time the period starts.</returns>
        private DateTime GetDateTimeStartOfPeriod(Periods period)
        {
            if (period == Periods.Today)
            {
                return DateTime.Today;
            }
            else if (period == Periods.ThisMonth)
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            else if (period == Periods.ThisYear)
            {
                return new DateTime(DateTime.Now.Year, 1, 1);
            }
            else if (period == Periods.Lifetime)
            {
                return DateTime.MinValue;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(period), $"{nameof(period)} is not defined.");
            }
        }

        #endregion

    }
}
