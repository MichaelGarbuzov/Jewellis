using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsletterController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public NewsletterController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/Newsletter
        public async Task<IActionResult> Index(string query)
        {
            List<NewsletterSubscriber> subscribers = await _dbContext.NewsletterSubscribers.Where(s => (query == null) || s.EmailAddress.Contains(query)).OrderByDescending(s => s.DateJoined).ToListAsync();
            ViewData["SearchQuery"] = query;
            return View(subscribers);
        }

        // GET: /Admin/Newsletter/Remove/{id}
        public async Task<IActionResult> Remove(int id)
        {
            NewsletterSubscriber subscriber = await _dbContext.NewsletterSubscribers.FirstOrDefaultAsync(s => s.Id == id);
            if (subscriber == null)
                return NotFound();
            else
                return View(subscriber);
        }

        // POST: /Admin/Newsletter/Remove/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Remove")]
        public async Task<IActionResult> Remove_POST(int id)
        {
            NewsletterSubscriber subscriber = await _dbContext.NewsletterSubscribers.FindAsync(id);
            _dbContext.NewsletterSubscribers.Remove(subscriber);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
