namespace day04.ViewModels
{
    public class ProductStatisticsViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalInventoryValue{ get; set; }
        public decimal AveragePrice { get; set; }
        public int TotalCategories { get; set; }
    }
}
