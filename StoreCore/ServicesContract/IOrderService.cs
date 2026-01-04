using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Dtos.Payments;
using StoreCore.Entities.Order;
using StoreCore.Entities.Payments;

namespace StoreCore.ServicesContract
{
    public interface IOrderService
    {
        Task<(Order order, PaymentResultDto payment)> CreateOrderAsync(
     string buyerEmail,
     int deliveryMethodId,
     string basketId,
     OrderAddress shippingAddress,
     PaymentMethodType paymentMethod,
     string? walletNumber = null
 );

        //Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, OrderAddress shippingAddress);
        Task<IEnumerable<Order?>> GetOrdersForUserAsync(string buyerEmail);
        Task<Order?> GetOrderByIdAsync(int id, string buyerEmail);
    }
}
