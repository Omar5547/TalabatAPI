using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specification.OrderSpec;

namespace Talabat.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IBasketRepository _basketRepo; // بيتعامل مع ال Redis ده اكنك بتتعامل مع Database
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentServices _paymentServices;

        public OrderServices(IBasketRepository basketRepo,
            IUnitOfWork unitOfWork, IPaymentServices paymentServices)
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentServices = paymentServices;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, int DeliveryMethodId, string basketId, Address shippingAddress)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            var OrderItems = new List<OrderItem>();
            if (basket?.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                { 
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var ProductItemOrdered = new ProductItemOrder(Product.Id, Product.Name, Product.PictureUrl);
                    var Orderitem = new OrderItem(ProductItemOrdered, Product.Price, item.Quantity);
                    OrderItems.Add(Orderitem);

                }
            }
            var SubTotal = OrderItems.Sum(item => item.Price * item.Quantity);
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);
            var Spec = new OrderWithPaymentIntentSpec(basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
            if (ExOrder != null) 
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentServices.CreatOrUpdatePaymentIntent(basketId);
            }
            var Order = new Order(buyerEmail, shippingAddress, DeliveryMethod, OrderItems, SubTotal,basket.PaymentIntentId);
           await _unitOfWork.Repository<Order>().Add(Order);
          
            var Result =  await _unitOfWork.CompleteAsync();
            if(Result<= 0) return null;
            return Order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return DeliveryMethods;
        }

        public async Task<Order> GetOrderByIdForSpecificuserAsync(int orderId, string buyerEmail)
        {
            var Spec = new OrderSpec(buyerEmail, orderId);  
       
            var Order =  await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
            return Order;
        }


        public Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var Spec = new OrderSpec(buyerEmail);
            var Orders = _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Orders;
        }
    }
}
