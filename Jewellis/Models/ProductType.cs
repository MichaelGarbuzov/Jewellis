using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a product type.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class ProductType
    {

        /// <summary>
        /// The id of the product type.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the product type.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        [Display(Name = "Date Last Modified")]
        public DateTime DateLastModified { get; set; }

    }
}
