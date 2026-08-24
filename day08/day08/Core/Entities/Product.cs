using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day08.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }

        public ICollection<InventoryTransaction> Transactions { get; set; }
            = new List<InventoryTransaction>();
    }
}
