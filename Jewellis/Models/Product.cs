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
        [Display(Name = "Name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the product.
        /// </summary>
        [Display(Name = "Description")]
        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// The path to the image of the product.
        /// </summary>
        [Display(Name = "Image")]
        [Required]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        /// <summary>
        /// The price for unit of the product.
        /// </summary>
        [Display(Name = "Price")]
        [Required]
        [Range(0, 100000)]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        /// <summary>
        /// Indicator if the product is available or not.
        /// </summary>
        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Date and time the product was added.
        /// </summary>
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        [Display(Name = "Date Last Modified")]
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
        public int? SaleId { get; set; }

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

        #region Public API

        /// <summary>
        /// Checks whther the product is currently on sale or not.
        /// </summary>
        /// <returns>Returns true if the product is currently on sale, otherwise false.</returns>
        public bool IsOnSaleNow()
        {
            return (this.Sale != null && this.Sale.DateStart < DateTime.Now && (this.Sale.DateEnd == null || DateTime.Now < this.Sale.DateEnd));
        }

        /// <summary>
        /// Gets the actual price of the product, after calculation of sale discount (if exists).
        /// </summary>
        /// <returns>Returns the actual price of the product, after calculation of sale discount (if exists).</returns>
        public double ActualPrice()
        {
            return (this.IsOnSaleNow() ? (this.Price * (1 - this.Sale.DiscountRate)) : this.Price);
        }

        #endregion

    }
}
