using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models.Helpers
{
    /// <summary>
    /// Represents the status of an order (<see cref="Order"/>).
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Marks an order as being payment processed.
        /// </summary>
        [Display(Name = "Payment Processing")]
        PaymentProcessing = 0,

        /// <summary>
        /// Marks an order as being packed for delivery.
        /// </summary>
        [Display(Name = "Packing")]
        Packing = 1,

        /// <summary>
        /// Marks an order as being shipped (packed and sent for delivery).
        /// </summary>
        [Display(Name = "Shipping")]
        Shipping = 2,

        /// <summary>
        /// Marks an order as closed (received by the customer).
        /// </summary>
        [Display(Name = "Received")]
        Closed = 3
    }
}
