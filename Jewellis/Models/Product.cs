using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        /*Foriegn Key*/
        public int CategoryId { get; set; }

        /*Foriegn Key*/
        public int TypeId { get; set; }

        /*Unique*/
        public int MyProperty { get; set; }

        public string Description  { get; set; }

        public string Image { get; set; }

        public double Price { get; set; }

        /*Foriegn Key*/
        public int? SaleId { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime LastModified { get; set; }
    }
}
