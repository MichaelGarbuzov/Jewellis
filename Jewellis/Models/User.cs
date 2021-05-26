using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /*Unique*/
        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string AddressStreet { get; set; }

        public string AddressCity { get; set; }

        public string AddressCountry { get; set; }

        public string AddressPostalCode { get; set; }

        public string Currency { get; set; }

        public string Theme { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime LastModified { get; set; }
    }
}
