using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Account.ViewModels.Newsletter
{
    public class SubscribeVM
    {

        [Display(Prompt = "Your email address...")]
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

    }
}
