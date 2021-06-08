using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Jewellis.Areas.Shop.ViewModels.Home;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class HomeController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public HomeController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("/shop")]
        public async Task<IActionResult> Index(IndexVM model)
        {
            // Gets the category id by the name (if searched):
            ProductCategory category = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Name.Equals(model.Category));
            if (category != null)
                model.CategoryId = category.Id;

            // Gets the type id by the name (if searched):
            ProductType type = await _dbContext.ProductTypes.FirstOrDefaultAsync(t => t.Name.Equals(model.Type));
            if (type != null)
                model.TypeId = type.Id;

            // Searches the products:
            var productsQuery = _dbContext.Products
                .Where(p => (model.CategoryId == null || p.CategoryId.Equals(model.CategoryId)) &&
                            (model.TypeId == null || p.TypeId.Equals(model.TypeId)) &&
                            (!model.Sale || (p.SaleId.HasValue && p.Sale.DateStart < DateTime.Now && (p.Sale.DateEnd == null || DateTime.Now < p.Sale.DateEnd))) &&
                            (model.Query == null || p.Name.Contains(model.Query) || p.Description.Contains(model.Query)) &&
                            (p.IsAvailable));
            switch (model.Sort)
            {
                case SortOptions.PriceLowToHigh:
                    productsQuery = productsQuery.OrderBy(p => (p.SaleId.HasValue ? (p.Price * (1 - p.Sale.DiscountRate)) : p.Price));
                    break;
                case SortOptions.PriceHighToLow:
                    productsQuery = productsQuery.OrderByDescending(p => (p.SaleId.HasValue ? (p.Price * (1 - p.Sale.DiscountRate)) : p.Price));
                    break;
                case SortOptions.NameAToZ:
                    productsQuery = productsQuery.OrderBy(p => p.Name);
                    break;
                case SortOptions.NameZToA:
                    productsQuery = productsQuery.OrderByDescending(p => p.Name);
                    break;
                case SortOptions.TimeNewsetFirst:
                default:
                    productsQuery = productsQuery.OrderByDescending(p => p.DateAdded);
                    break;
            }
            List<Product> products = await productsQuery.Include(p => p.Sale).ToListAsync();

            #region Filter by Price...

            // Gets the minimum and maximum prices in the list of products:
            var productPrices = products.Select(p => (p.SaleId.HasValue ? (p.Price * (1 - p.Sale.DiscountRate)) : (p.Price))).OrderBy(p => p);
            double minPrice = Math.Floor(productPrices.FirstOrDefault());
            double maxPrice = Math.Ceiling(productPrices.LastOrDefault());

            // Filters the price range:
            products = products.Where(p => (model.MinPrice == null || (p.SaleId.HasValue ? (p.Price * (1 - p.Sale.DiscountRate) >= model.MinPrice.Value) : (p.Price >= model.MinPrice.Value))) &&
                                           (model.MaxPrice == null || (p.SaleId.HasValue ? (p.Price * (1 - p.Sale.DiscountRate) <= model.MaxPrice.Value) : (p.Price <= model.MaxPrice.Value)))).ToList();

            #endregion

            ViewData["Products"] = products;
            ViewData["ProductCategories"] = await _dbContext.ProductCategories.ToListAsync();
            ViewData["ProductTypes"] = await _dbContext.ProductTypes.ToListAsync();
            ViewData["ProductsMinPrice"] = minPrice;
            ViewData["ProductsMaxPrice"] = maxPrice;
            return View(model);
        }

    }
}
