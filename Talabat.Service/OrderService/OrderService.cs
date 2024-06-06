using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Order_Specs;
using Talabat.Core.Services.Contract;

namespace Talabat.Application.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		public OrderService(
			IBasketRepository basketRepo,
			IUnitOfWork unitOfWork,
			IPaymentService paymentService
			)
        {
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
			_paymentService = paymentService;
		}
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			// 1.Get Basket From Baskets Repo
			var basket = await _basketRepo.GetBasketAsync(basketId);

			// 2. Get Selected Items at Basket From Products Repo
			var orderItems = new List<OrderItem>();	
			
			if(basket?.Items.Count > 0)
			{
				var productRepository = _unitOfWork.Repository<Product>();

				foreach (var item in basket.Items)
                {
					var product = await productRepository.GetByIdAsync(item.Id);
					var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
					orderItems.Add(orderItem);
                }
            }

			// 3. Calculate SubTotal
			var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

			// 4. Get Delivery Method From DeliveryMethods Repo
			var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

			var orderRepo = _unitOfWork.Repository<Order>();

			var spec = new OrderWithPaymentIntentSpecification(basket?.PaymentIntentId);

			var existingOrder = await orderRepo.GetAllWithSpecAsync(spec);

			if(existingOrder is not null)
			{
				orderRepo.Delete((Order)existingOrder);
				await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			}

			// 5. Create Order
			var order = new Order(
				buyerEmail: buyerEmail,
				shippingAddress: shippingAddress,
				deliveryMethodId: deliveryMethodId,
				items: orderItems,
				subtotal: subtotal,
				paymentIntentId: basket?.PaymentIntentId ?? ""
				);

			orderRepo.AddAsync(order);
			// 6. Save To Database [TODO]

			var result = await _unitOfWork.CompleteAsync();
			if (result <= 0) return null;

			return order;
		}

		public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
		{
			var ordersRepo = _unitOfWork.Repository<Order>();
			var spec = new OrderSpecifications(buyerEmail);
			var orders = await ordersRepo.GetAllWithSpecAsync(spec);

			return orders;
		}

		public Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
		{
			var ordersRepo = _unitOfWork.Repository<Order>();
			var orderSpec = new OrderSpecifications(orderId, buyerEmail);
			var order = ordersRepo.GetByIdWithSpecAsync(orderSpec);	

			return order;
		}
		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
		   => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
	}
}
