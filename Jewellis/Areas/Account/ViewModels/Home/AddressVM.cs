using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Account.ViewModels.Home
{
    public class AddressVM
    {

        /// <summary>
        /// The street of the address.
        /// </summary>
        [Display(Name = "Street *")]
        [Required(ErrorMessage = "Street is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        public string Street { get; set; }

        /// <summary>
        /// The postal code of the address.
        /// </summary>
        [Display(Name = "Postal Code *")]
        [Required(ErrorMessage = "Postal code is required.")]
        [DataType(DataType.PostalCode, ErrorMessage = "Invalid postal code.")]
        [StringLength(30, ErrorMessage = "Maximum length allowed is 30 characters.")]
        public string PostalCode { get; set; }

        /// <summary>
        /// The city of the address.
        /// </summary>
        [Display(Name = "City *")]
        [Required(ErrorMessage = "City is required.")]
        [StringLength(30, ErrorMessage = "Maximum length allowed is 30 characters.")]
        public string City { get; set; }

        /// <summary>
        /// The country of the address.
        /// </summary>
        [Display(Name = "Country *")]
        [Required(ErrorMessage = "Country is required.")]
        [StringLength(30, ErrorMessage = "Maximum length allowed is 30 characters.")]
        public string Country { get; set; }

    }
}
