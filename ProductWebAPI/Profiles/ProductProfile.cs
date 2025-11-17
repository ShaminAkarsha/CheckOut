using AutoMapper;
using ProductWebAPI.Dtos;
using ProductWebAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductWebAPI.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<ProductSyncDto, Product>();
        }
    }
}
