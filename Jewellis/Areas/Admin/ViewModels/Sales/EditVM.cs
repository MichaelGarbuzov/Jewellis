using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Sales
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
        /// The name of the sale.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name *")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckNameEditAvailability", "Sales", "Admin", AdditionalFields = nameof(CurrentName), ErrorMessage = "Name already taken.")]
        public new string Name { get; set; }

    }
}
