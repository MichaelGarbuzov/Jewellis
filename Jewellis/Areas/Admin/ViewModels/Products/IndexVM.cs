using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Products
{
    public class IndexVM
    {

        [Display(Prompt = "Enter keywords here...")]
        public string Query { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Type")]
        public int? TypeId { get; set; }

        [Display(Name = "On Sale")]
        public int? SaleId { get; set; }

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
