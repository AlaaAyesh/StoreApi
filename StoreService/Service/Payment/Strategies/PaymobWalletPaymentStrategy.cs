using System.Threading.Tasks;
using StoreCore.Dtos.Orders;
using StoreCore.Dtos.Payments;
using StoreCore.ServicesContract;

namespace StoreService.Service.Payment.Strategies
{
    // ✅ استخدم Primary Constructor بالشكل الصحيح
    public class PaymobWalletPaymentStrategy(IPaymentService paymentService) : IPaymentStrategy
    {
        // ✅ مفيش داعي تعمل تعريف متكرر للـ field (C# 12 بتدخله تلقائيًا)
        private readonly IPaymentService _paymentService = paymentService;

        // ✅ async method لأن فيها await
        public async Task<PaymentResultDto> ProcessPaymentAsync(
            string basketId,
            string buyerEmail,
            AddressOrderDto billingAddress,
            string? walletNumber = null)
        {
            // ✅ تحقق من وجود رقم المحفظة
            if (string.IsNullOrWhiteSpace(walletNumber))
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Wallet number is required for wallet payments"
                };
            }

            // ✅ استدعاء خدمة الدفع من Paymob عبر IPaymentService
            var result = await _paymentService.CreateWalletPaymentAsync(
                basketId,
                buyerEmail,
                billingAddress,
                walletNumber
            );

            return result;
        }
    }
}
