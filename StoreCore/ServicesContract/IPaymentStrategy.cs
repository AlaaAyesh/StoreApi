using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Dtos.Orders;
using StoreCore.Dtos.Payments;

namespace StoreCore.ServicesContract
{
    public interface IPaymentStrategy
    {
        Task<PaymentResultDto> ProcessPaymentAsync(string basketId, string buyerEmail, AddressOrderDto billingAddress, string? walletNumber = null);
    }

}
