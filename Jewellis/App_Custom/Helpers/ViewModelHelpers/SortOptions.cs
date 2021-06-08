using System.ComponentModel.DataAnnotations;

namespace Jewellis.App_Custom.Helpers.ViewModelHelpers
{
    /// <summary>
    /// Represents an enumeration for sort options.
    /// </summary>
    public enum SortOptions
    {
        /// <summary>
        /// Sort option for time: newest first.
        /// </summary>
        [Display(Name = "Time: Newset First")]
        TimeNewsetFirst,

        /// <summary>
        /// Sort option for price: low to high.
        /// </summary>
        [Display(Name = "Price: Low to High")]
        PriceLowToHigh,

        /// <summary>
        /// Sort option for price: high to low.
        /// </summary>
        [Display(Name = "Price: High to Low")]
        PriceHighToLow,

        /// <summary>
        /// Sort option for name: A to Z.
        /// </summary>
        [Display(Name = "Name: A to Z")]
        NameAToZ,

        /// <summary>
        /// Sort option for name: Z to A.
        /// </summary>
        [Display(Name = "Name: Z to A")]
        NameZToA
    }
}
