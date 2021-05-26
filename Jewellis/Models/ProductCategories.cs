using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class ProductCategories
    {
        [Key]
        public int Id { get; set; }

        /*Unique*/
        public string Name { get; set; }

        public DateTime LastModified { get; set; }
    }
}
