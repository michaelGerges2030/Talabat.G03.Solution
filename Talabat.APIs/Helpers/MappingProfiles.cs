using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles: Profile
	{

		public MappingProfiles()
        {

			CreateMap<Product, ProductToReturnDto>()
                .ForMember(P => P.Brand, O => O.MapFrom(S => S.Brand.Name))
                .ForMember(P => P.Category, O => O.MapFrom(S => S.Category.Name))
                .ForMember(P => P.PictureUrl, O => O.MapFrom<ProductPictureUrlResolve>());

			CreateMap<CustomerBasketDto, CustomerBasket>();
			CreateMap<BasketItemDto, BasketItem>();

			CreateMap<AddressDto, Address>().ReverseMap();

			CreateMap<Order, OrderToReturnDto>()
				.ForMember(d => d.DeliveryMethod, O => O.MapFrom(s => s.DeliveryMethod.ShortName))
				.ForMember(d => d.DeliveryMethodCost, O => O.MapFrom(s => s.DeliveryMethod.Cost));

			CreateMap<OrderItem, OrderItemDto>()
				.ForMember(d => d.ProductId, O => O.MapFrom(s => s.Product.ProductId))
				.ForMember(d => d.ProductName, O => O.MapFrom(s => s.Product.ProductName))
				.ForMember(d => d.ProductUrl, O => O.MapFrom(s => s.Product.ProductUrl))
				.ForMember(d => d.ProductUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());
		
		}

    }
}
