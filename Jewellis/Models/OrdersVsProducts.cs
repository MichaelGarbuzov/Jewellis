namespace Jewellis.Models
{
    public class OrdersVsProducts
    {
        /*Foreign Key*/
        public int OrderId { get; set; }

        /*Foreign Key*/
        public int ProductId { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }

        public float DiscountRate { get; set; }
    }
}
