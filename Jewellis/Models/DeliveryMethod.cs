using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a delivery method for an order (<see cref="Order"/>).
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class DeliveryMethod
    {

        /// <summary>
        /// The id of the delivery method.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the delivery method.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the delivery method.
        /// </summary>
        [Display(Name = "Description")]
        [Required]
        [StringLength(50)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// The price of the delivery method.
        /// </summary>
        [Display(Name = "Price")]
        [Required]
        [Range(0, 100000)]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        [Display(Name = "Date Last Modified")]
        public DateTime DateLastModified { get; set; }

    }
}
