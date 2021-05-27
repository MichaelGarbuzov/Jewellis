using Jewellis.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents an order.
    /// </summary>
    public class Order
    {

        /// <summary>
        /// The id of the order.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The billing name of the order.
        /// </summary>
        public string BillingName { get; set; }

        /// <summary>
        /// The billing phone of the order.
        /// </summary>
        public string BillingPhone { get; set; }

        /// <summary>
        /// The shipping name of the order.
        /// </summary>
        public string ShippingName { get; set; }

        /// <summary>
        /// The shipping phone of the order.
        /// </summary>
        public string ShippingPhone { get; set; }

        /// <summary>
        /// The customer note for the order.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The current status of the order.
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Date and time the order was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        #region Relationships

        /// <summary>
        /// The user created the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public User User { get; set; }

        /// <summary>
        /// The delivery method of the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public DeliveryMethod DeliveryMethod { get; set; }

        /// <summary>
        /// The billing address of the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public Address BillingAddress { get; set; }

        /// <summary>
        /// The shipping address of the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// The list of products in the order.
        /// </summary>
        /// <remarks>[Relationship: Many-to-Many]</remarks>
        public List<OrderProduct> OrderProducts { get; set; }

        #endregion

    }
}
