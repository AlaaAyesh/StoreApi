using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Entities.Order;

namespace StoreCore.Specificatons.Orders
{
    public class OrderSpecificationWithPayementIntentId :BaseSpecifications<Order, int>
    {
        public OrderSpecificationWithPayementIntentId(string paymentIntentId)
            : base(o => o.PaymentIntentId == paymentIntentId)
        {
            Includes.Add(o => o.OrderItems);
            Includes.Add(o => o.DeliveryMethod);
        }


    }

  
}
