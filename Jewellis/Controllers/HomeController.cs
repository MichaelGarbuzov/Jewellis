using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
using Jewellis.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public IActionResult Stores()
        {
            // TODO: From DB
            List<Branch> branches = new List<Branch>();
            branches.Add(new Branch()
            {
                Id = 1,
                Name = "Tel Aviv - Dizengoff Center",
                Adrress = "Dizengoff St. 50, Tel Aviv-Yafo",
                PhoneNumber = "03-1234567",
                OpeningHours = "Sunday-Thursday: 10:00-20:00\r\nFriday and Holidays: 10:00-14:00\r\nSaturday: Closed",
                LocationLatitude = 32.0748402,
                LocationLongitude = 34.7751358,
                DateLastModified = DateTime.Now
            });
            branches.Add(new Branch()
            {
                Id = 2,
                Name = "Tel Aviv - Shenkin Street",
                Adrress = "Shenkin St. 12, Tel Aviv-Yafo",
                PhoneNumber = "03-1234567",
                OpeningHours = "Sunday-Thursday: 10:00-20:00\r\nFriday and Holidays: 10:00-14:00\r\nSaturday: Closed",
                LocationLatitude = 32.0687168,
                LocationLongitude = 34.7780078,
                DateLastModified = DateTime.Now
            });
            branches.Add(new Branch()
            {
                Id = 3,
                Name = "Rishon Lezion - HaZahav Mall",
                Adrress = "Herzl 21, Rishon Lezion",
                PhoneNumber = "03-1234567",
                OpeningHours = "Sunday-Thursday: 10:00-20:00\r\nFriday and Holidays: 10:00-14:00\r\nSaturday: Closed",
                LocationLatitude = 31.9913925,
                LocationLongitude = 34.7764307,
                DateLastModified = DateTime.Now
            });
            return View(branches);
        }

    }
}
