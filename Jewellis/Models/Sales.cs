using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class Sales
    {
        [Key]
        public int Id { get; set; }

        /*(Unique)*/
        public string Name { get; set; }
        
        public float DiscountRate { get; set; }

        public DateTime? DateExpired { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastModified { get; set; }
    }
}
