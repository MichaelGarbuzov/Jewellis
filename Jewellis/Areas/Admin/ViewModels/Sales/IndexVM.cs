using System;
using System.ComponentModel.DataAnnotations;
using Jewellis.App_Custom.Validations;

namespace Jewellis.Areas.Admin.ViewModels.Sales
{
    public class IndexVM
    {

        [Display(Prompt = "Enter keywords here...")]
        public string Query { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time.")]
        public DateTime? DuringStart { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time.")]
        [DateTimeGreaterThan(nameof(DuringStart), ErrorMessage = "Date and time must be greater.")]
        public DateTime? DuringEnd { get; set; }

        public int? PageSize { get; set; }
        public int Page { get; set; }

        public IndexVM()
        {
            // Pagination defaults:
            PageSize = 10;
            Page = 1;
        }

    }
}
