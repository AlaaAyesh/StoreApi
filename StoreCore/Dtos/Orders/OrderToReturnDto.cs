using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Entities.Order;

namespace StoreCore.Dtos.Orders
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public AddressOrderDto ShipToAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryMethodPrice { get; set; }
        public decimal Subtotal { get; set; }
        public IEnumerable<OrderItemDto> OrderItems { get; set; }
        public decimal Total { get; set; }

        public string Status { get; set; }
        public string? PaymentIntentId { get; set; }=string.Empty;
    }
}
