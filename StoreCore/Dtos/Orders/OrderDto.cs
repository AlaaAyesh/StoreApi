using StoreCore.Entities.Payments; // ✅ علشان نستخدم PaymentMethodType

namespace StoreCore.Dtos.Orders
{
    public class OrderDto
    {
        public string BasketId { get; set; } = string.Empty;
        public int DeliveryMethodId { get; set; }
        public AddressOrderDto ShipToAddress { get; set; }

        // 💳 تحديد وسيلة الدفع (كارت / محفظة / كاش)
        public PaymentMethodType PaymentMethod { get; set; }
        public string? WalletNumber { get; set; }

    }
}
