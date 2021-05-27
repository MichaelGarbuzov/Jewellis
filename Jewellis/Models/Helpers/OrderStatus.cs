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
        PaymentProcessing = 0,

        /// <summary>
        /// Marks an order as being packed for delivery.
        /// </summary>
        Packing = 1,

        /// <summary>
        /// Marks an order as being shipped (packed and sent for delivery).
        /// </summary>
        Shipping = 2,

        /// <summary>
        /// Marks an order as closed (received by the customer).
        /// </summary>
        Closed = 3
    }
}
