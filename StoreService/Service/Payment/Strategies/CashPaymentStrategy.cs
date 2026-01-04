using System.Threading.Tasks;
using StoreCore.Dtos.Orders;
using StoreCore.Dtos.Payments;
using StoreCore.ServicesContract;

namespace StoreService.Service.Payment.Strategies
{
    public class CashPaymentStrategy : IPaymentStrategy
    {
        private readonly IPaymentService _paymentService;

        public CashPaymentStrategy(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<PaymentResultDto> ProcessPaymentAsync(
            string basketId,
            string buyerEmail,
            AddressOrderDto billingAddress,
            string? walletNumber = null)  // ✅ أضفنا البراميتر الرابع
        {
            return _paymentService.CreateCashPaymentAsync(basketId, buyerEmail, billingAddress);
        }
    }
}
