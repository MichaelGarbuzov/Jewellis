using Jewellis.Models.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Users
{
    public class IndexVM
    {

        [Display(Prompt = "Enter keywords here...")]
        public string Query { get; set; }

        [Display(Name = "Role")]
        public UserRole? Role { get; set; }

    }
}
