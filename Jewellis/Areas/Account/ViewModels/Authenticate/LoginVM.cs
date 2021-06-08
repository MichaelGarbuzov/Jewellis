using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Account.ViewModels.Authenticate
{
    public class LoginVM
    {

        /// <summary>
        /// The email address of the user.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Email Address", Prompt = "Email Address")]
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        [Display(Name = "Password", Prompt = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
