using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a sale.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Sale
    {

        /// <summary>
        /// The id of the sale.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the sale.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// The discount rate (value between 0 to 1) of the sale.
        /// </summary>
        [Range(0, 1)]
        public double DiscountRate { get; set; }

        /// <summary>
        /// Date and time the sale starts.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Date and time the sale ends.
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Date and time the sale created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        public DateTime DateLastModified { get; set; }

    }
}
