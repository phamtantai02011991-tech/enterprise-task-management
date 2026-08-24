namespace day08.Core.Entities
{
    public enum TransactionType
    {
        Import = 1,
        Export = 2
    }
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
