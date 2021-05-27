namespace Jewellis.Models
{
    /// <summary>
    /// Represents a product in an order.
    /// </summary>
    public class OrderProduct
    {

        /// <summary>
        /// The price per unit of the product in the order.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// The quantity of the product in the order.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The discount rate (value between 0 to 1) of the product in the order.
        /// </summary>
        public double DiscountRate { get; set; }

        #region Relationships

        /// <summary>
        /// The order associated.
        /// </summary>
        /// <remarks>[Relationship: Many-to-Many]</remarks>
        public Order Order { get; set; }

        /// <summary>
        /// The product associated.
        /// </summary>
        /// <remarks>[Relationship: Many-to-Many]</remarks>
        public Product Product { get; set; }

        #endregion

    }
}
