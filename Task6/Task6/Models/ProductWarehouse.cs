namespace Task6.Models
{
    public class ProductWarehouse
    {
        public int ProductWarehouseId { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int IdOrder { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
