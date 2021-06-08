using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Account.ViewModels.Authenticate
{
    public class RegisterVM
    {

        /// <summary>
        /// The first name of the user.
        /// </summary>
        [Display(Name = "First Name", Prompt = "First Name")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [Display(Name = "Last Name", Prompt = "Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
        public string LastName { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Email Address", Prompt = "Email Address")]
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address.")]
        [Remote("CheckEmailAvailability", "Authenticate", "Account", ErrorMessage = "Email is already registered.")]
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

        /// <summary>
        /// The confirm password of the user.
        /// </summary>
        [Display(Name = "Confirm Password", Prompt = "Confirm Password")]
        [Required(ErrorMessage = "Confirm password is required.")]
        [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Subscribe our newsletter")]
        public bool SubscribeNewsletter { get; set; }

    }
}
