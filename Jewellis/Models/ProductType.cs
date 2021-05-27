using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a product type.
    /// </summary>
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
        public string Name { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        public DateTime DateLastModified { get; set; }

    }
}
