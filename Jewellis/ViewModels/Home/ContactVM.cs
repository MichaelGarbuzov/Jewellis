using System.ComponentModel.DataAnnotations;

namespace Jewellis.ViewModels.Home
{
    public class ContactVM
    {

        /// <summary>
        /// The first name of the contact opener.
        /// </summary>
        [Display(Name = "First Name *", Prompt = "First Name *")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(25, ErrorMessage = "Maximum length allowed is 25 characters.")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the contact opener.
        /// </summary>
        [Display(Name = "Last Name *", Prompt = "Last Name *")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(25, ErrorMessage = "Maximum length allowed is 25 characters.")]
        public string LastName { get; set; }

        /// <summary>
        /// The email address of the contact opener.
        /// </summary>
        [Display(Name = "Email Address *", Prompt = "Email Address *")]
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The subject of the contact.
        /// </summary>
        [Display(Name = "Subject *", Prompt = "Subject *")]
        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        public string Subject { get; set; }

        /// <summary>
        /// The body text of the contact.
        /// </summary>
        [Display(Name = "Message", Prompt = "Message")]
        [StringLength(500, ErrorMessage = "Maximum length allowed is 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

    }
}
