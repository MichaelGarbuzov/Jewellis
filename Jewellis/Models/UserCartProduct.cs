using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a user's cart product.
    /// </summary>
    public class UserCartProduct
    {

        /// <summary>
        /// Date and time the product was added to the cart.
        /// </summary>
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        #region Relationships

        /// <summary>
        /// The id of the user related to the cart.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int UserId { get; set; }

        /// <summary>
        /// The user related to the cart.
        /// </summary>
        /// <remarks>[Relationship: One-to-One], [Unique]</remarks>
        public User User { get; set; }

        /// <summary>
        /// The id of the product related to the user's cart.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int ProductId { get; set; }

        /// <summary>
        /// The product related to the user's cart.
        /// </summary>
        /// <remarks>[Relationship: One-to-One], [Unique]</remarks>
        public Product Product { get; set; }

        #endregion

    }
}
