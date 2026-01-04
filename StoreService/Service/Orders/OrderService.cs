using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreCore;
using StoreCore.Dtos.Orders;
using StoreCore.Dtos.Payments;
using StoreCore.Entities;
using StoreCore.Entities.Order;
using StoreCore.Entities.Payments;
using StoreCore.ServicesContract;
using StoreCore.Specificatons.Orders;
using StoreService.Service.Baskets;
using StoreService.Service.Payment; // يحتوي على IPaymentStrategyFactory
// تأكد أن PaymentMethodType موجود في نفس Namespace أو استخدم الـ using المناسب

namespace StoreService.Service.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketService _basketService;
        private readonly IPaymentStrategyFactory _paymentStrategyFactory;

        public OrderService(
            IUnitOfWork unitOfWork,
            IBasketService basketService,
            IPaymentStrategyFactory paymentStrategyFactory)
        {
            _unitOfWork = unitOfWork;
            _basketService = basketService;
            _paymentStrategyFactory = paymentStrategyFactory;
        }

        public async Task<(Order order, PaymentResultDto payment)> CreateOrderAsync(
            string buyerEmail,
            int deliveryMethodId,
            string basketId,
            OrderAddress shippingAddress,
            PaymentMethodType paymentMethod,
            string? walletNumber = null
            )
        {
            // 1. جلب السلة والتأكد منها
            var basket = await _basketService.GetBasketAsync(basketId);
            if (basket == null || basket.Items == null || !basket.Items.Any())
            {
                return (null, new PaymentResultDto { IsSuccess = false, Message = "Basket is empty" });
            }

            // 2. تجهيز عناصر الأوردر
            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product, int>().GetByIdAsync(item.ProductId);
                if (product == null) continue;

                var itemOrdered = new ProductItemOrder
                {
                    ProductItemId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl
                };

                orderItems.Add(new OrderItem(itemOrdered, product.Price, item.Quantity));
            }

            if (orderItems.Count == 0)
            {
                return (null, new PaymentResultDto { IsSuccess = false, Message = "No valid items in basket" });
            }

            // 3. استرجاع طريقة التوصيل
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(deliveryMethodId);
            if (deliveryMethod == null)
            {
                return (null, new PaymentResultDto { IsSuccess = false, Message = "Invalid delivery method" });
            }

            // 4. حساب subtotal
            var subtotal = orderItems.Sum(i => i.Price * i.Quantity);

            // 5. تحويل عنوان الشحن الى DTO للبيلينج
            var billingAddressDto = new AddressOrderDto
            {
                FirstName = shippingAddress.FirstName,
                LastName = shippingAddress.LastName,
                Street = shippingAddress.Street,
                City = shippingAddress.City,
                State = shippingAddress.State,
                Country = "EG"
            };

            // 6. استدعاء موفر الدفع المناسب عبر الفاكتوري
            IPaymentStrategy paymentStrategy;
            try
            {
                paymentStrategy = _paymentStrategyFactory.GetStrategy(paymentMethod);
            }
            catch (Exception ex)
            {
                return (null, new PaymentResultDto { IsSuccess = false, Message = $"Payment method error: {ex.Message}" });
            }

            PaymentResultDto paymentResult;
            try
            {
                paymentResult = await paymentStrategy.ProcessPaymentAsync(basketId, buyerEmail, billingAddressDto, walletNumber);
            }
            catch (Exception ex)
            {
                // لو حصل خطأ أثناء الاتصال بموفر الدفع نرجع رسالة مفهومة
                return (null, new PaymentResultDto { IsSuccess = false, Message = $"Payment provider error: {ex.Message}" });
            }

            if (!paymentResult.IsSuccess)
            {
                return (null, paymentResult); // ارجع نتيجة الدفع (خطأ/رسالة) كما هي
            }

            // 7. إنشاء كيان الأوردر وحفظه في DB
            var order = new Order
            {
                BuyerEmail = buyerEmail,
                ShipToAddress = shippingAddress,
                DeliveryMethod = deliveryMethod,
                OrderItems = orderItems,
                Subtotal = subtotal,
                PaymentIntentId = paymentResult.PaymobPaymentId
            };

            await _unitOfWork.Repository<Order, int>().AddAsync(order);
            await _unitOfWork.CompleteAsync();

            // 8. حذف السلة بعد إنشاء الطلب
            await _basketService.DeleteBasketAsync(basketId);

            // 9. ارجع الأوردر ونتيجة الدفع
            return (order, paymentResult);
        }

        public async Task<Order?> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrderSpecification(id, buyerEmail);
            return await _unitOfWork.Repository<Order, int>().GetByIdWithSpecificationAsync(spec);
        }

        public async Task<IEnumerable<Order?>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecification(buyerEmail);
            var orders = await _unitOfWork.Repository<Order, int>().GetAllWithSpecificationAsync(spec);
            if (orders == null || !orders.Any())
                return Enumerable.Empty<Order?>();
            return orders;
        }
    }
}
