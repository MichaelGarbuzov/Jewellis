using Jewellis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Products
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
        /// The name of the product.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name *")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckNameEditAvailability", "Products", "Admin", AdditionalFields = nameof(CurrentName), ErrorMessage = "Name already taken.")]
        public string Name { get; set; }

        /// <summary>
        /// The description of the product.
        /// </summary>
        [Display(Name = "Description *")]
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Maximum length allowed is 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// The path to the image of the product.
        /// </summary>
        [HiddenInput]
        [Required(ErrorMessage = "Image is required.")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        [Display(Name = "Image *")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// The price for unit of the product.
        /// </summary>
        [Display(Name = "Price ($) *")]
        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 100000, ErrorMessage = "Must be between 0 to 100000.")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        /// <summary>
        /// Indicator if the product is available or not.
        /// </summary>
        [Display(Name = "Available")]
        [Required(ErrorMessage = "Availability is required.")]
        public bool IsAvailable { get; set; }

        #region Relationships

        /// <summary>
        /// The category id of the product.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        [Display(Name = "Category *")]
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        /// <summary>
        /// The type id of the product.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        [Display(Name = "Type *")]
        [Required(ErrorMessage = "Type is required.")]
        public int TypeId { get; set; }

        /// <summary>
        /// The current sale id on the product.
        /// </summary>
        /// <remarks>[Foreign Key]</remarks>
        [Display(Name = "Sale")]
        public int? SaleId { get; set; }

        #endregion

    }
}
