using System;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a user's wishlist product.
    /// </summary>
    public class UserWishlistProduct
    {

        /// <summary>
        /// Date and time the product was added to the wishlist.
        /// </summary>
        public DateTime DateAdded { get; set; }

        #region Relationships

        /// <summary>
        /// The product related to the user's wishlist.
        /// </summary>
        /// <remarks>[Relationship: One-to-One], [Unique]</remarks>
        public Product Product { get; set; }

        #endregion

    }
}
