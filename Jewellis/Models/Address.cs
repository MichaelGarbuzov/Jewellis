using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents an address.
    /// </summary>
    public class Address
    {

        /// <summary>
        /// The id of the address.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The street of the address.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Street { get; set; }

        /// <summary>
        /// The postal code of the address.
        /// </summary>
        [Required]
        [DataType(DataType.PostalCode)]
        [StringLength(30)]
        public string PostalCode { get; set; }

        /// <summary>
        /// The city of the address.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string City { get; set; }

        /// <summary>
        /// The country of the address.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string Country { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        public DateTime DateLastModified { get; set; }

    }
}
