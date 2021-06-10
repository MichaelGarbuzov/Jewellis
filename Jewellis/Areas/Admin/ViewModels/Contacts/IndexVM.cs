using Jewellis.Models.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Contacts
{
    public class IndexVM
    {

        [Display(Prompt = "Enter keywords here...")]
        public string Query { get; set; }

        [Display(Name = "Status")]
        public ContactStatus? Status { get; set; }

    }
}
