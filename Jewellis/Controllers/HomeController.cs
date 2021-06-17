using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Jewellis.Services;
using Jewellis.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Controllers
{
    public class HomeController : Controller
    {
        private readonly JewellisDbContext _dbContext;
        private readonly UserIdentityService _userIdentity;
        private readonly ClientCurrencyService _clientCurrency;

        public HomeController(JewellisDbContext dbContext, UserIdentityService userIdentity, ClientCurrencyService clientCurrency)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
            _clientCurrency = clientCurrency;
        }

        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/terms")]
        public IActionResult Terms()
        {
            return View();
        }

        [Route("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/help")]
        public IActionResult Help()
        {
            return View();
        }

        [Route("/contact")]
        [HttpGet]
        public async Task<IActionResult> Contact(string subject)
        {
            ContactVM model = new ContactVM()
            {
                Subject = subject
            };
            User user = await _userIdentity.GetCurrentAsync();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.EmailAddress = user.EmailAddress;
            }
            return View(model);
        }

        [Route("/contact")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Contact contact = new Contact()
            {
                Name = $"{model.FirstName} {model.LastName}",
                EmailAddress = model.EmailAddress,
                Subject = model.Subject,
                Body = model.Body,
                Status = ContactStatus.Pending
            };
            _dbContext.Contacts.Add(contact);
            await _dbContext.SaveChangesAsync();
            TempData["Success"] = true;
            return RedirectToAction(nameof(Contact));
        }

        [Route("/stores")]
        public async Task<IActionResult> Stores()
        {
            List<Branch> branches = await _dbContext.Branches.OrderBy(b => b.Name).ToListAsync();
            return View(branches);
        }

        #region AJAX Actions

        [Route("/main-search")]
        [AjaxOnly]
        public async Task<IActionResult> MainSearch(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Json(false);

            List<Product> products = await _dbContext.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .Take(5)
                .Include(p => p.Sale)
                .ToListAsync();

            var bindProductsJSON = products.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                imagePath = p.ImagePath,
                price = _clientCurrency.GetPriceAndDisplay(p.ActualPrice())
            }).ToList();

            return Json(bindProductsJSON);
        }

        #endregion

    }
}
