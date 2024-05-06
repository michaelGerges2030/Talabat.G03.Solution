using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.Application.OrderService
{
	public class OrderService : IOrderService
	{
		Task<Order> IOrderService.CreateOrderAsync(string buyerEmail, string basketId, string deliveryMethodId, Address shippingAddress)
		{
			throw new NotImplementedException();
		}

		Task<IReadOnlyList<Order>> IOrderService.GetOrdersForUserAsync(string buyerEmail)
		{
			throw new NotImplementedException();
		}

		Task<Order> IOrderService.GetOrderByIdForUserAsync(string buyerEmail, int orderId)
		{
			throw new NotImplementedException();
		}
		Task<IReadOnlyList<DeliveryMethod>> IOrderService.GetDeliveryMethodsAsync()
		{
			throw new NotImplementedException();
		}
	}
}
