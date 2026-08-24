using day04.Models;

namespace day04.ViewModels
{
    public class ProductIndexViewModel
    {
        public string? Keyword { get; set; }
        public List<Product> Products { get; set; } = new();
        public ProductStatisticsViewModel Statistics { get; set; } = new();
    }
}
