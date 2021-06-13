using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a client's cart (for both authenticated and unauthenticated users).
    /// </summary>
    public class ClientCart
    {

        /// <summary>
        /// The id of the client's cart.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Date and time the client's cart was created.
        /// </summary>
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        #region Relationships

        /// <summary>
        /// The products in the client's cart.
        /// </summary>
        /// <remarks>[Relationship: One-to-Many]</remarks>
        public List<ClientCartProduct> Products { get; set; }

        #endregion

    }
}
