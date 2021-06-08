using Jewellis.Data;
using Jewellis.Models;
using Jewellis.Models.Helpers;
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
    public class ContactsController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public ContactsController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/Contacts
        public async Task<IActionResult> Index(string query, ContactStatus? status)
        {
            List<Contact> contacts = await _dbContext.Contacts
                .Where(c => ((query == null) || c.Name.Contains(query) || c.EmailAddress.Contains(query) || c.Subject.Contains(query) || c.Body.Contains(query)) &&
                            ((status == null) || c.Status == status.Value))
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync();
            ViewData["SearchQuery"] = query;
            ViewData["SearchStatus"] = status;
            return View(contacts);
        }

        // GET: /Admin/Contacts/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();
            else
                return View(contact);
        }

        // GET: /Admin/Contacts/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();
            else
                return View(contact);
        }

        // POST: /Admin/Contacts/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Contact contact = await _dbContext.Contacts.FindAsync(id);
            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Contacts/UpdateStatus/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ContactStatus status)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();

            // Binds the view model:
            contact.Status = status;
            contact.DateLastModified = DateTime.Now;

            _dbContext.Contacts.Update(contact);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
