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
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// The email address of the contact opener.
        /// </summary>
        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The subject of the contact.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Subject { get; set; }

        /// <summary>
        /// The body text of the contact.
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        /// <summary>
        /// The current status of the contact.
        /// </summary>
        public ContactStatus Status { get; set; }

        /// <summary>
        /// Date and time the contact was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        public DateTime DateLastModified { get; set; }

    }
}
