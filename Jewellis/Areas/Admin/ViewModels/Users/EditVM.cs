using Jewellis.Models.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Users
{
    public class EditVM
    {

        /// <summary>
        /// The id of the user.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        [HiddenInput]
        public int Id { get; set; }

        [HiddenInput]
        public string CurrentEmail { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        public string LastName { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckEmailEditAvailability", "Users", "Admin", AdditionalFields = nameof(CurrentEmail), ErrorMessage = "Email already in use.")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The role of the user.
        /// </summary>
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required.")]
        public UserRole Role { get; set; }

    }
}
