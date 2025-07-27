using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specification.OrderSpec
{
    public class OrderSpec :BaseSpecifications<Order>
    {
        public OrderSpec(string email):base(O=>O.BuyerEmail==email) 
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
            AddOrderByDescending(O => O.OrderDate);

        }
        public OrderSpec(String email , int OrderId) :base(O=>O.BuyerEmail==email && O.Id==OrderId )
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);

        }
        
    }
}
