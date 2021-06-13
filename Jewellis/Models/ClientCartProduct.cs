using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a client's cart product.
    /// </summary>
    public class ClientCartProduct
    {

        /// <summary>
        /// The quantity of the product in the cart.
        /// </summary>
        [Display(Name = "Quantity")]
        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        /// <summary>
        /// Date and time the product was added to the cart.
        /// </summary>
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        #region Relationships

        /// <summary>
        /// The id of the client's cart related.
        /// </summary>
        /// <remarks>[Foreign Key], [Primary Key with <see cref="ProductId"/>]</remarks>
        public int ClientCartId { get; set; }

        /// <summary>
        /// The client cart related.
        /// </summary>
        /// <remarks>[Relationship: One-to-One], [Unique with Product]</remarks>
        public ClientCart ClientCart { get; set; }

        /// <summary>
        /// The id of the product related to the client's cart.
        /// </summary>
        /// <remarks>[Foreign Key], [Primary Key with <see cref="ClientCartId"/>]</remarks>
        public int ProductId { get; set; }

        /// <summary>
        /// The product related to the client's cart.
        /// </summary>
        /// <remarks>[Relationship: One-to-One], [Unique with ClientCart]</remarks>
        public Product Product { get; set; }

        #endregion

    }
}
