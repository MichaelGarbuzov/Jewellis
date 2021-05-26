using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class ProductTypes
    {
        [Key]
        public int Id { get; set; }

        /*Unique*/
        public string Name { get; set; }

        public string Image { get; set; }

        public DateTime LastModified { get; set; }
    }
}
