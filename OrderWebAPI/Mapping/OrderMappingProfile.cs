using AutoMapper;
using OrderWebAPI.Dtos;
using OrderWebAPI.Models;

namespace OrderWebAPI.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // Order mappings
  CreateMap<Order, OrderDto>();
  CreateMap<OrderDto, Order>();
         CreateMap<CreateOrderDto, Order>();

        // Order Item mappings
      CreateMap<OrderItem, OrderItemDto>();
  CreateMap<OrderItemDto, OrderItem>();

   // Order Payment mappings
  CreateMap<OrderPayment, OrderPaymentDto>();
    CreateMap<OrderPaymentDto, OrderPayment>();

       // Order Summary mapping
     CreateMap<Order, OrderSummaryDto>()
   .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count))
    .ForMember(dest => dest.ProductNames, opt => opt.MapFrom(src => src.Items.Select(i => i.ProductName).ToList()));
        }
    }
}