using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Services
{
    public interface IOrderServices
    {
        Task<Order?> CreateOrderAsync ( string BuerEmail,int DeliveryMethodId, string basketId, Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdForSpecificuserAsync(int orderId, string buyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
    }
}
