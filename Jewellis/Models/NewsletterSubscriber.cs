using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a newsletter subscriber.
    /// </summary>
    [Index(nameof(EmailAddress), IsUnique = true)]
    public class NewsletterSubscriber
    {

        /// <summary>
        /// The id of the newsletter subscriber.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The email address of the newsletter subscriber.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Email Address")]
        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Date and time the subscriber joined to the newsletter.
        /// </summary>
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }

    }
}
