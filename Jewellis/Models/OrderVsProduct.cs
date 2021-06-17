using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a product in an order.
    /// </summary>
    public class OrderVsProduct
    {

        /// <summary>
        /// The price per unit of the product in the order.
        /// </summary>
        [Display(Name = "Unit Price")]
        [Required]
        [Range(0, 100000)]
        [DataType(DataType.Currency)]
        public double UnitPrice { get; set; }

        /// <summary>
        /// The quantity of the product in the order.
        /// </summary>
        [Display(Name = "Quantity")]
        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        /// <summary>
        /// The discount rate (value between 0 to 1) of the product in the order.
        /// </summary>
        [Display(Name = "Discount Rate")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Range(0, 1)]
        public double? DiscountRate { get; set; }

        #region Relationships

        /// <summary>
        /// The id of the order associated.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int OrderId { get; set; }

        /// <summary>
        /// The order associated.
        /// </summary>
        /// <remarks>[Relationship: Many-to-Many]</remarks>
        public Order Order { get; set; }

        /// <summary>
        /// The id of the product associated.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int ProductId { get; set; }

        /// <summary>
        /// The product associated.
        /// </summary>
        /// <remarks>[Relationship: Many-to-Many]</remarks>
        public Product Product { get; set; }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the actual price per unit the customer paid for the product, after calculation of discount (if exists).
        /// </summary>
        /// <returns>Returns the actual price per unit the customer paid for the product, after calculation of discount (if exists).</returns>
        public double ActualPricePerUnit()
        {
            return (this.DiscountRate.HasValue ? (this.UnitPrice * (1 - this.DiscountRate.Value)) : this.UnitPrice);
        }

        #endregion

    }
}
