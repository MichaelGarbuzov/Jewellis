using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.ProductTypes
{
    public class CreateVM
    {

        /// <summary>
        /// The name of the type.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name *")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckNameAvailability", "ProductTypes", "Admin", ErrorMessage = "Name already taken.")]
        public string Name { get; set; }

    }
}
