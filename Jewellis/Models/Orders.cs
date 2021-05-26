using Jewellis.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Models
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        /*Foreign Key*/
        public int UserId { get; set; }

        /*Foreign Key*/
        public int DeliveryMethodId { get; set; }

        public string BillingName { get; set; }

        public string BillingPhone { get; set; }

        public string BillingAddressStreet { get; set; }

        public string BillingAddressPostalCode { get; set; }

        public string BillingAddressCity { get; set; }

        public string BillingAddressCountry { get; set; }

        public string ShippingName { get; set; }

        public string ShippingPhone { get; set; }

        public string ShippingAddressStreet { get; set; }

        public string ShippingAddressPostalCode { get; set; }

        public string ShippingAddressCity { get; set; }

        public string ShippingAddressCountry { get; set; }

        public string Note { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
