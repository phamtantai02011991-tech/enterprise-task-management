using System.ComponentModel.DataAnnotations;
using day08.Core.Entities;

namespace day08.Core.ViewModels
{
    public class StockTransactionViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn sản phẩm hợp lệ")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại giao dịch")]
        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;

        public string? Note { get; set; }
    }
}
