using Jewellis.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Admin.ViewModels.Orders
{
    public class IndexVM
    {

        [Display(Name = "Status")]
        public OrderStatus? Status { get; set; }

        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        public DateTime? DateCreated { get; set; }

        [Display(Name = "Order #", Prompt = "Type order id here...")]
        public string OrderId { get; set; }

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
