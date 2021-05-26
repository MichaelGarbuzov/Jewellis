using System;

namespace Jewellis.Models
{
    public class UserWishlist
    {
        /*Foreign Key*/
        public int UserId { get; set; }

        /*Foreign Key*/
        public int ProductId { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
