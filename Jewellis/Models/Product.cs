using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a product.
    /// </summary>
    public class Product
    {

        /// <summary>
        /// The id of the product.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        public string Name { get; set; }

        /// <summary>
        /// The description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The path to the image of the product.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// The price for unit of the product.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Indicator if the product is available or not.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Date and time the product was added.
        /// </summary>
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        public DateTime DateLastModified { get; set; }

        #region Relationships

        /// <summary>
        /// The category of the product.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public ProductCategory Category { get; set; }

        /// <summary>
        /// The type of the product.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public ProductType Type { get; set; }

        /// <summary>
        /// The current sale on the product.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public Sale Sale { get; set; }

        #endregion

    }
}
