using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Jewellis.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jewellis.Controllers
{
    public class HomeController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public HomeController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
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
        public IActionResult Contact(string subject)
        {
            ViewData["Subject"] = subject;
            return View();
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

    }
}
