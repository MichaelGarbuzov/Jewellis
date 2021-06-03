using Jewellis.App_Custom.ActionFilters;
using Jewellis.Areas.Account.ViewModels.Newsletter;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Jewellis.Areas.Account.Controllers
{
    [Area("Account")]
    public class NewsletterController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public NewsletterController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Checks the email is not already subscribed:
            if (await _dbContext.NewsletterSubscribers.AnyAsync(s => s.EmailAddress.Equals(model.EmailAddress)) == false)
            {
                NewsletterSubscriber subscriber = new NewsletterSubscriber()
                {
                    EmailAddress = model.EmailAddress
                };
                _dbContext.NewsletterSubscribers.Add(subscriber);
                await _dbContext.SaveChangesAsync();
            }
            return Json(new { Message = "Great! We will let you know about awesome jewelry!" });
        }

    }
}
