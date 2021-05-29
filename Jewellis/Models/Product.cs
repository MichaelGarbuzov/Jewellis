using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a product.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(CategoryId), nameof(TypeId), nameof(Id))]
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
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the product.
        /// </summary>
        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// The path to the image of the product.
        /// </summary>
        [Required]
        [StringLength(400)]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        /// <summary>
        /// The price for unit of the product.
        /// </summary>
        [Required]
        [Range(0, 100000)]
        [DataType(DataType.Currency)]
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
        /// The category id of the product.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int CategoryId { get; set; }

        /// <summary>
        /// The category of the product.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public ProductCategory Category { get; set; }

        /// <summary>
        /// The type id of the product.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int TypeId { get; set; }

        /// <summary>
        /// The type of the product.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public ProductType Type { get; set; }

        /// <summary>
        /// The current sale id on the product.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int SaleId { get; set; }

        /// <summary>
        /// The current sale on the product.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public Sale Sale { get; set; }

        /// <summary>
        /// The list of orders related to the product.
        /// </summary>
        /// <remarks>[Relationship: Many-to-Many]</remarks>
        public List<OrderVsProduct> OrderProducts { get; set; }

        #endregion

    }
}
