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
        /// The id of the user related to the wishlist.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int UserId { get; set; }

        /// <summary>
        /// The user related to the wishlist.
        /// </summary>
        /// <remarks>[Relationship: One-to-One], [Unique]</remarks>
        public User User { get; set; }

        /// <summary>
        /// The id of the product related to the user's wishlist.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        public int ProductId { get; set; }

        /// <summary>
        /// The product related to the user's wishlist.
        /// </summary>
        /// <remarks>[Relationship: One-to-One], [Unique]</remarks>
        public Product Product { get; set; }

        #endregion

    }
}
