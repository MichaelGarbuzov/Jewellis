using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a sale.
    /// </summary>
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
        public string Name { get; set; }

        /// <summary>
        /// The discount rate (value between 0 to 1) of the sale.
        /// </summary>
        public double DiscountRate { get; set; }

        /// <summary>
        /// Date and time the sale starts.
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Date and time the sale ends.
        /// </summary>
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
