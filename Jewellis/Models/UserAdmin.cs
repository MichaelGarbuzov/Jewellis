using System;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents an admin user.
    /// </summary>
    public class UserAdmin
    {

        /// <summary>
        /// Date and time the user was added as an admin.
        /// </summary>
        public DateTime DateAdded { get; set; }

        #region Relationships

        /// <summary>
        /// The user related.
        /// </summary>
        /// <remarks>[Relationship: One-to-One]</remarks>
        public User User { get; set; }

        #endregion

    }
}
