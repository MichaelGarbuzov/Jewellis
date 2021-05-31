using Jewellis.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a contact in the site.
    /// </summary>
    public class Contact
    {

        /// <summary>
        /// The id of the contact.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the contact opener.
        /// </summary>
        [Display(Name = "Name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// The email address of the contact opener.
        /// </summary>
        [Display(Name = "Email Address")]
        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The subject of the contact.
        /// </summary>
        [Display(Name = "Subject")]
        [Required]
        [StringLength(50)]
        public string Subject { get; set; }

        /// <summary>
        /// The body text of the contact.
        /// </summary>
        [Display(Name = "Body")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        /// <summary>
        /// The current status of the contact.
        /// </summary>
        [Display(Name = "Status")]
        public ContactStatus Status { get; set; }

        /// <summary>
        /// Date and time the contact was created.
        /// </summary>
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        [Display(Name = "Date Last Modified")]
        public DateTime DateLastModified { get; set; }

    }
}
