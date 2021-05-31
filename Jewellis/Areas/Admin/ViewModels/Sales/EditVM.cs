using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Sales
{
    public class EditVM
    {

        /// <summary>
        /// The id of the sale.
        /// </summary>
        /// <remarks>[Primary Key], [Identity]</remarks>
        [Key]
        [HiddenInput]
        public int Id { get; set; }

        [HiddenInput]
        public string CurrentName { get; set; }

        /// <summary>
        /// The name of the sale.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckNameEditAvailability", "Sales", "Admin", AdditionalFields = nameof(CurrentName), ErrorMessage = "Name already taken.")]
        public string Name { get; set; }

        /// <summary>
        /// The discount rate (value between 0 to 1) of the sale.
        /// </summary>
        [Display(Name = "Discount Rate")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        [Required(ErrorMessage = "Discount rate is required.")]
        [Range(0, 1, ErrorMessage = "Must be between 0 to 1.")]
        public double DiscountRate { get; set; }

        /// <summary>
        /// Date and time the sale starts.
        /// </summary>
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time.")]
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Date and time the sale ends.
        /// </summary>
        [Display(Name = "End Date")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time.")]
        public DateTime? DateEnd { get; set; }

    }
}
