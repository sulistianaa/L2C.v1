namespace InventoryGudang.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int SupplierId { get; set; }
        public string? TransactionType { get; set; }
        public DateTime DateIn  { get; set; }
        public DateTime DateOut { get; set; }  
        public Supplier? Supplier { get; set; }
    }
}