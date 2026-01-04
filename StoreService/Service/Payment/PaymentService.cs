// Path: StoreService/Service/Payment/PaymentService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using StoreCore;
using StoreCore.Dtos.Orders;
using StoreCore.Dtos.Payments;
using StoreCore.Entities.Order;
using StoreCore.Entities.Payments;
using StoreCore.ServicesContract;
using static StoreService.Service.Payment.PaymobClient;

namespace StoreService.Service.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketService _basketService;
        private readonly PaymobClient _paymobClient;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IBasketService basketService, PaymobClient paymobClient, IUnitOfWork unitOfWork)
        {
            _basketService = basketService;
            _paymobClient = paymobClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentResultDto> CreateOrUpdatePaymobPaymentAsync(
            string basketId,
            string buyerEmail,
            AddressOrderDto billingAddress)
        {
            var basket = await _basketService.GetBasketAsync(basketId);
            var count = basket?.Items?.Count ?? 0;
            if (count == 0)
                return new PaymentResultDto { IsSuccess = false, Message = "Basket is empty" };

            var total = basket.Items.Sum(i => i.Price * i.Quantity);

            // حاول نجيب billing من آخر أوردر لو مش موجود
            if (billingAddress == null)
            {
                var orderRepo = _unitOfWork.Repository<Order, int>();
                var userOrders = await orderRepo.GetAllAsync();
                var lastOrder = userOrders
                    .Where(o => o.BuyerEmail == buyerEmail)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault();

                if (lastOrder?.ShipToAddress != null)
                {
                    billingAddress = new AddressOrderDto
                    {
                        FirstName = lastOrder.ShipToAddress.FirstName,
                        LastName = lastOrder.ShipToAddress.LastName,
                        Street = lastOrder.ShipToAddress.Street,
                        City = lastOrder.ShipToAddress.City,
                        State = lastOrder.ShipToAddress.State,
                        Country = "EG"
                    };
                }
            }

            if (billingAddress == null)
            {
                billingAddress = new AddressOrderDto
                {
                    FirstName = "Customer",
                    LastName = "Unknown",
                    Street = "N/A",
                    City = "Cairo",
                    State = "Cairo",
                    Country = "EG"
                };
            }

            // إنشاء في Paymob (card)
            var token = await _paymobClient.CreatePaymentAsync(total, billingAddress);

            var payment = new StoreCore.Entities.Payments.Payment
            {
                BuyerEmail = buyerEmail,
                BasketId = basketId,
                Amount = total,
                PaymobPaymentId = token,
                Status = PaymentStatus.Pending
            };

            await _unitOfWork.Repository<StoreCore.Entities.Payments.Payment, int>().AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            return new PaymentResultDto
            {
                IsSuccess = true,
                PaymobPaymentId = token,
                Message = "Payment created successfully",
                PaymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_paymobClient.IframeId}?payment_token={token}"
            };
        }

        public async Task<bool> HandlePaymobWebhookAsync(string paymobPaymentId, string status)
        {
            var paymentRepo = _unitOfWork.Repository<StoreCore.Entities.Payments.Payment, int>();
            var payments = await paymentRepo.GetAllAsync();
            var payment = payments.FirstOrDefault(p => p.PaymobPaymentId == paymobPaymentId);
            if (payment == null) return false;

            if (Enum.TryParse<PaymentStatus>(status, true, out var parsed))
                payment.Status = parsed;
            else
                payment.Status = PaymentStatus.Failed;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<PaymentResultDto> CreateWalletPaymentAsync(
    string basketId,
    string buyerEmail,
    AddressOrderDto billingAddress,
    string walletNumber)
        {
            var basket = await _basketService.GetBasketAsync(basketId);
            var count = basket?.Items?.Count ?? 0;
            if (count == 0)
                return new PaymentResultDto { IsSuccess = false, Message = "Basket is empty" };

            var total = basket.Items.Sum(i => i.Price * i.Quantity);

            if (string.IsNullOrWhiteSpace(walletNumber))
                return new PaymentResultDto { IsSuccess = false, Message = "Wallet number is required for wallet payments" };

            if (billingAddress == null)
            {
                billingAddress = new AddressOrderDto
                {
                    FirstName = "Customer",
                    LastName = "Wallet",
                    Street = "N/A",
                    City = "Cairo",
                    State = "Cairo",
                    Country = "EG"
                };
            }

            WalletPaymentResult result;
            try
            {
                result = await _paymobClient.CreateWalletPaymentAsync(total, billingAddress, walletNumber);
            }
            catch (Exception ex)
            {
                // سجّل السبب وارجع نتيجة فاشلة بدل silent empty url
                // ضع Logger إذا رغبت (أضف ILogger<PaymentService> في الكونستركتور)
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = $"Paymob wallet error: {ex.Message}"
                };
            }

            if (result == null || string.IsNullOrEmpty(result.RedirectUrl))
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Paymob did not return a redirect url for wallet payment."
                };
            }

            var payment = new StoreCore.Entities.Payments.Payment
            {
                BuyerEmail = buyerEmail,
                BasketId = basketId,
                Amount = total,
                PaymobPaymentId = result.PaymentToken,
                Status = PaymentStatus.Pending
            };

            await _unitOfWork.Repository<StoreCore.Entities.Payments.Payment, int>().AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            return new PaymentResultDto
            {
                IsSuccess = true,
                PaymobPaymentId = result.PaymentToken,
                PaymentUrl = result.RedirectUrl,
                Message = "Wallet payment initiated successfully"
            };
        }

        public async Task<PaymentResultDto> CreateCashPaymentAsync(string basketId, string buyerEmail, AddressOrderDto billingAddress)
        {
            // منطق الدفع النقدي (بدون بوابة إلكترونية)
            // ممكن هنا تسجلي الطلب في قاعدة البيانات كـ "مدفوع عند الاستلام"

            // مثال:
            return new PaymentResultDto
            {
                IsSuccess = true,
                Message = "Cash payment order created successfully.",
                PaymentUrl = null
            };
        }


    }
}
