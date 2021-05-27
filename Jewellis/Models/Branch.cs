using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class Branch
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Adrress { get; set; }

        public string PhoneNumber { get; set; }

        public string OpeningHours { get; set; }

        public double LocationAltitude { get; set; }

        public double LocationLongitude { get; set; }

        public DateTime DateLastModified { get; set; }

    }
}
