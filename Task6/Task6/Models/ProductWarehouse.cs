namespace Task6.Models
{
    public class ProductWarehouse
    {
        public int IdProductWarehouse { get; set; }
        public int IdWarehouse { get; set; }
        public int ProductID { get; set; }
        public int IdOrder { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
