using Jewellis.App_Custom.ActionFilters;
using Jewellis.Areas.Admin.ViewModels.Products;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private const string IMAGES_FOLDER_PATH = "/files/images/products";

        private readonly JewellisDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(JewellisDbContext dbContext, IWebHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
        }

        // GET: /Admin/Products
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<Product> products = await _dbContext.Products
                .Where(p => ((model.Query == null) || p.Name.Contains(model.Query) || p.Description.Contains(model.Query)) &&
                            ((model.CategoryId == null) || p.CategoryId == model.CategoryId) &&
                            ((model.TypeId == null) || p.TypeId == model.TypeId) &&
                            ((model.SaleId == null) || p.SaleId == model.SaleId))
                .OrderByDescending(p => p.DateLastModified)
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .ToListAsync();
            ViewData["ProductsModel"] = products;
            ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name));
            ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name));
            ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name));
            return View(model);
        }

        // GET: /Admin/Products/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Product product = await _dbContext.Products
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();
            else
                return View(product);
        }

        // GET: /Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name));
            ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name));
            ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name));
            return View(new CreateVM()
            {
                IsAvailable = true
            });
        }

        // POST: /Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name), model.CategoryId);
                ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name), model.TypeId);
                ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name), model.SaleId);
                return View(model);
            }

            // Saves the product image:
            string imagePath = await SaveImage(model.ImageFile);

            Product product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                ImagePath = imagePath,
                Price = model.Price,
                IsAvailable = model.IsAvailable,
                CategoryId = model.CategoryId,
                TypeId = model.TypeId,
                SaleId = model.SaleId
            };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Products/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Product product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name), product.CategoryId);
                ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name), product.TypeId);
                ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name), product.SaleId);
                return View(new EditVM()
                {
                    Id = product.Id,
                    CurrentName = product.Name,
                    Name = product.Name,
                    Description = product.Description,
                    ImagePath = product.ImagePath,
                    Price = product.Price,
                    IsAvailable = product.IsAvailable,
                    CategoryId = product.CategoryId,
                    TypeId = product.TypeId,
                    SaleId = product.SaleId
                });
            }
        }

        // POST: /Admin/Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            Product product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name), product.CategoryId);
                ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name), product.TypeId);
                ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name), product.SaleId);
                return View(model);
            }

            // Checks if the image was changed, in order to update files:
            if (model.ImageFile != null)
            {
                DeleteImage(product.ImagePath);
                model.ImagePath = await SaveImage(model.ImageFile);
            }
            else
            {
                model.ImagePath = product.ImagePath;
            }

            // Binds the view model:
            product.Name = model.Name;
            product.Description = model.Description;
            product.ImagePath = model.ImagePath;
            product.Price = model.Price;
            product.IsAvailable = model.IsAvailable;
            product.CategoryId = model.CategoryId;
            product.TypeId = model.TypeId;
            product.SaleId = model.SaleId;
            product.DateLastModified = DateTime.Now;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Products/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _dbContext.Products
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();
            else
                return View(product);
        }

        // POST: /Admin/Products/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Product product = await _dbContext.Products.FindAsync(id);
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            // Deletes the product image:
            DeleteImage(product.ImagePath);

            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.Products.AnyAsync(p => p.Name.Equals(name)) == false);
            return Json(isNameAvailable);
        }

        [AjaxOnly]
        public async Task<JsonResult> CheckNameEditAvailability(string name, string currentName)
        {
            // Checks if the name did not change in the edit:
            if (string.Equals(name, currentName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }
            // Otherwise, name was changed so checks availability:
            else
            {
                bool isNameAvailable = (await _dbContext.Products.AnyAsync(p => p.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves the specified product image file to its folder, and returns the relative image path saved.
        /// </summary>
        /// <param name="imageFile">The product image file.</param>
        /// <returns>Returns the relative image path saved.</returns>
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            // Generates a unique file name by the current time (year-month-day-hour-minute-second-millisecond):
            string fileName = (DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(imageFile.FileName));

            string relativeImagePath = $"{IMAGES_FOLDER_PATH}/{fileName}";
            string absoluteImagePath = Path.Combine(_hostEnvironment.WebRootPath, relativeImagePath.TrimStart('/'));

            using (FileStream fileStream = new FileStream(absoluteImagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return relativeImagePath;
        }

        /// <summary>
        /// Deletes the specified product image path (relative path).
        /// </summary>
        /// <param name="imagePath">The image relative path to delete.</param>
        private void DeleteImage(string imagePath)
        {
            string absoluteImagePath = Path.Combine(_hostEnvironment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(absoluteImagePath))
            {
                System.IO.File.Delete(absoluteImagePath);
            }
        }

        #endregion

    }
}
