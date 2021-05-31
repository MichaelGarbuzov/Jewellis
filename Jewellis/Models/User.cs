using Jewellis.Models.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a user (customer).
    /// </summary>
    [Index(nameof(EmailAddress), IsUnique = true)]
    public class User
    {

        /// <summary>
        /// The id of the user.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        [Display(Name = "First Name")]
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [Display(Name = "Last Name")]
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Email Address")]
        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        [Display(Name = "Password")]
        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// The role of the user.
        /// </summary>
        [Display(Name = "Role")]
        public UserRole Role { get; set; }

        /// <summary>
        /// The phone number of the user.
        /// </summary>
        [Display(Name = "Phone Number")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The currency preference of the user.
        /// </summary>
        [Display(Name = "Currency")]
        [Required]
        [StringLength(5)]
        public string Currency { get; set; }

        /// <summary>
        /// The theme preference of the user.
        /// </summary>
        [Display(Name = "Theme")]
        [Required]
        [StringLength(20)]
        public string Theme { get; set; }

        /// <summary>
        /// Date and time the user was registered.
        /// </summary>
        [Display(Name = "Date Registered")]
        public DateTime DateRegistered { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        [Display(Name = "Date Last Modified")]
        public DateTime DateLastModified { get; set; }

        #region Relationships

        /// <summary>
        /// The address id of the user.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int? AddressId { get; set; }

        /// <summary>
        /// The address of the user.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public Address Address { get; set; }

        /// <summary>
        /// The cart products of the user.
        /// </summary>
        /// <remarks>[Relationship: One-to-Many]</remarks>
        public List<UserCartProduct> Cart { get; set; }

        /// <summary>
        /// The wishlist products of the user.
        /// </summary>
        /// <remarks>[Relationship: One-to-Many]</remarks>
        public List<UserWishlistProduct> Wishlist { get; set; }

        /// <summary>
        /// The list of orders created by the user.
        /// </summary>
        /// <remarks>[Relationship: One-to-Many]</remarks>
        public List<Order> Orders { get; set; }

        #endregion

    }
}
