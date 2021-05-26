using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class DeliveryMethods
    {
        [Key]
        public int Id { get; set; }

        /*Unique*/
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public DateTime LastModified { get; set; }
    }
}
