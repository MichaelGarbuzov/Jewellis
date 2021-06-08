using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Products
{
    public class EditVM : CreateVM
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
        /// The name of the product.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name *")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckNameEditAvailability", "Products", "Admin", AdditionalFields = nameof(CurrentName), ErrorMessage = "Name already taken.")]
        public new string Name { get; set; }

        /// <summary>
        /// The path to the image of the product.
        /// </summary>
        [HiddenInput]
        [Required(ErrorMessage = "Image is required.")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        [Display(Name = "Change Image")]
        public new IFormFile ImageFile { get; set; }

    }
}
