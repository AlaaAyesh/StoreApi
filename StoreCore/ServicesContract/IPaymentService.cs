// Path: StoreCore/ServicesContract/IPaymentService.cs
using System.Threading.Tasks;
using StoreCore.Dtos.Orders;
using StoreCore.Dtos.Payments;

namespace StoreCore.ServicesContract
{
    public interface IPaymentService
    {
        /// <summary>
        /// Create or update a Paymob payment (card flow). billingAddress can be null.
        /// Returns PaymentResultDto with payment token (PaymobPaymentId) and PaymentUrl (iframe url).
        /// </summary>
        Task<PaymentResultDto> CreateOrUpdatePaymobPaymentAsync(
            string basketId,
            string buyerEmail,
            AddressOrderDto billingAddress = null
        );

        /// <summary>
        /// Create a wallet payment (mobile wallet) via Paymob. Returns token + redirect url.
        /// </summary>
        Task<PaymentResultDto> CreateWalletPaymentAsync(
           string basketId,
           string buyerEmail,
           AddressOrderDto billingAddress,
           string walletNumber
       );

        /// <summary>
        /// Handle Paymob webhook: update Payment status by paymobPaymentId.
        /// </summary>
        Task<bool> HandlePaymobWebhookAsync(string paymobPaymentId, string status);
        Task<PaymentResultDto> CreateCashPaymentAsync(string basketId, string buyerEmail, AddressOrderDto billingAddress);



    }
}
