using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCore.Entities.Order
{
    public class Order : BaseEntity<int>
    {
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public OrderAddress ShipToAddress { get; set; }

        public int DeliveryMethodId { get; set; }// FK
        public DeliveryMethod DeliveryMethod { get; set; }

        public decimal Subtotal { get; set; }
        public decimal GetTotal()
        {
            return Subtotal + (DeliveryMethod?.Price ?? 0);
        }

        public string PaymentIntentId { get; set; } // Stripe Payment Intent Id
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


    }
}
