using Jewellis.Models.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents an order.
    /// </summary>
    [Index(nameof(UserId), nameof(Id))]
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
        [Display(Name = "Billing Name")]
        [Required]
        [StringLength(50)]
        public string BillingName { get; set; }

        /// <summary>
        /// The billing phone of the order.
        /// </summary>
        [Display(Name = "Billing Phone")]
        [Required]
        [StringLength(20)]
        public string BillingPhone { get; set; }

        /// <summary>
        /// The shipping name of the order.
        /// </summary>
        [Display(Name = "Shipping Name")]
        [StringLength(50)]
        public string ShippingName { get; set; }

        /// <summary>
        /// The shipping phone of the order.
        /// </summary>
        [Display(Name = "Shipping Phone")]
        [StringLength(20)]
        public string ShippingPhone { get; set; }

        /// <summary>
        /// The customer note for the order.
        /// </summary>
        [Display(Name = "Note")]
        [StringLength(100)]
        public string Note { get; set; }

        /// <summary>
        /// The current status of the order.
        /// </summary>
        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Date and time the order was created.
        /// </summary>
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        #region Relationships

        /// <summary>
        /// The id of the user created the order.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int UserId { get; set; }

        /// <summary>
        /// The user created the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public User User { get; set; }

        /// <summary>
        /// The delivery method id of the order.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int DeliveryMethodId { get; set; }

        /// <summary>
        /// The delivery method of the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public DeliveryMethod DeliveryMethod { get; set; }

        /// <summary>
        /// The billing address id of the order.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int BillingAddressId { get; set; }

        /// <summary>
        /// The billing address of the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        [ForeignKey(nameof(BillingAddressId))]
        public Address BillingAddress { get; set; }

        /// <summary>
        /// The shipping address id of the order.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int? ShippingAddressId { get; set; }

        /// <summary>
        /// The shipping address of the order.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        [ForeignKey(nameof(ShippingAddressId))]
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// The list of products in the order.
        /// </summary>
        /// <remarks>[Relationship: Many-to-Many]</remarks>
        public List<OrderVsProduct> OrderProducts { get; set; }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the subtotal price (regular price before discounts) the customer paid for the order.
        /// </summary>
        /// <returns>Returns the subtotal price (regular price before discounts) the customer paid for the order.</returns>
        public double GetSubtotal()
        {
            double subtotal = 0;
            foreach (var orderProduct in this.OrderProducts)
            {
                subtotal += (orderProduct.UnitPrice * orderProduct.Quantity);
            }
            return subtotal;
        }

        /// <summary>
        /// Gets the total discount the customer received on the order.
        /// </summary>
        /// <returns>Returns the total discount the customer received on the order.</returns>
        public double GetDiscount()
        {
            double discount = 0;
            foreach (var orderProduct in this.OrderProducts)
            {
                discount += ((orderProduct.UnitPrice - orderProduct.ActualPricePerUnit()) * orderProduct.Quantity);
            }
            return discount;
        }

        /// <summary>
        /// Gets the total price (actual price after discounts) the customer paid for the order.
        /// </summary>
        /// <returns>Returns the total price (actual price after discounts) the customer paid for the order.</returns>
        public double GetTotalPrice()
        {
            double total = 0;
            foreach (var orderProduct in this.OrderProducts)
            {
                total += (orderProduct.ActualPricePerUnit() * orderProduct.Quantity);
            }
            return (total + this.DeliveryMethod.Price);
        }

        #endregion

    }
}
