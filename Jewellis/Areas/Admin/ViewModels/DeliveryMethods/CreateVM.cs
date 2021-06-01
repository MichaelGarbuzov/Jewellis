using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.DeliveryMethods
{
    public class CreateVM
    {

        /// <summary>
        /// The name of the delivery method.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name *")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckNameAvailability", "DeliveryMethods", "Admin", ErrorMessage = "Name already taken.")]
        public string Name { get; set; }

        /// <summary>
        /// The description of the delivery method.
        /// </summary>
        [Display(Name = "Description *")]
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// The price of the delivery method.
        /// </summary>
        [Display(Name = "Price ($) *")]
        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 100000, ErrorMessage = "Price must be between 0 to 100000.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid price value.")]
        public double Price { get; set; }

    }
}
