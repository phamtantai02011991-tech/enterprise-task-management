using AutoMapper;
using day08.Core.Entities;
using day08.Core.ViewModels;

namespace day08.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Product, ProductViewModel>().ReverseMap();
            CreateMap<InventoryTransaction, StockTransactionViewModel>().ReverseMap();
        }
    }
}
