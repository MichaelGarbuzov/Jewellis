using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    /// <summary>
    /// Represents a branch information.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Branch
    {

        /// <summary>
        /// The id of the branch.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the branch.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// The address of the branch.
        /// </summary>
        [Display(Name = "Address")]
        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        /// <summary>
        /// The phone number of the branch.
        /// </summary>
        [Display(Name = "Phone Number")]
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The opening hours of the branch.
        /// </summary>
        [Display(Name = "Opening Hours")]
        [Required]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        public string OpeningHours { get; set; }

        /// <summary>
        /// The latitude of the branch location.
        /// </summary>
        [Display(Name = "Location Latitude")]
        [Required]
        public double LocationLatitude { get; set; }

        /// <summary>
        /// The longitude of the branch location.
        /// </summary>
        [Display(Name = "Location Longitude")]
        [Required]
        public double LocationLongitude { get; set; }

        /// <summary>
        /// Date and time of the last modify on the record.
        /// </summary>
        [Display(Name = "Date Last Modified")]
        public DateTime DateLastModified { get; set; }

    }
}
