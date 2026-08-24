namespace day08.Core.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public bool OutOfStock => StockQuantity <= 0;
        public bool IsLowStock => StockQuantity > 0 && StockQuantity <= 5;   
    }
}
