using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Shop.ViewModels.Home
{
    public class IndexVM
    {

        /// <summary>
        /// Search by category name, for outside calls without knowing the category id.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Search by type name, for outside calls without knowing the type id.
        /// </summary>
        public string Type { get; set; }

        [HiddenInput]
        public int? CategoryId { get; set; }

        [HiddenInput]
        public int? TypeId { get; set; }

        [HiddenInput]
        public double? MinPrice { get; set; }

        [HiddenInput]
        public double? MaxPrice { get; set; }

        [Display(Prompt = "Type keywords here...")]
        public string Query { get; set; }

        [Display(Name = "On Sale")]
        public bool Sale { get; set; }

        public SortOptions Sort { get; set; }

        public int? PageSize { get; set; }

        public int Page { get; set; }

        public IndexVM()
        {
            // Pagination defaults:
            PageSize = 12;
            Page = 1;
        }

    }
}
