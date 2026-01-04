// Path: StoreCore/Entities/Payments/Payment.cs
using System;

namespace StoreCore.Entities.Payments
{
    public class Payment : BaseEntity<int>
    {
        public string BuyerEmail { get; set; } = string.Empty;
        public string BasketId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymobPaymentId { get; set; } = string.Empty; // token or id from Paymob
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
