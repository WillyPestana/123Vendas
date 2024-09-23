using _123Vendas.Core.Dtos;
using _123Vendas.Core.Entities;

namespace _123Vendas.API.Configurations
{
    public class AutoMapperConfig : AutoMapper.Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Venda, VendaDto>().ReverseMap();
            CreateMap<ItemVenda, ItemVendaDto>().ReverseMap();
        }
    }
}
