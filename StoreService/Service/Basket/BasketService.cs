using StoreCore.Entities;
using StoreCore.RepositoriesContract;
using StoreCore.ServicesContract;

namespace StoreService.Service.Baskets
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;

        public BasketService(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        // 🔹 استرجاع السلة عن طريق buyerId
        public async Task<CustomerBasket?> GetBasketAsync(string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId)) return null;

            var basket = await _basketRepository.GetBasketByBuyerIdAsync(buyerId);
            return basket ?? new CustomerBasket { Id = buyerId };
        }

        // 🔹 حفظ أو تحديث السلة
        public async Task<CustomerBasket?> SaveOrUpdateBasketAsync(CustomerBasket basket)
        {
            if (basket == null || string.IsNullOrEmpty(basket.Id))
                throw new ArgumentException("Invalid basket or buyer ID.");

            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);
            return updatedBasket;
        }

        // 🔹 حذف السلة بالكامل
        public async Task<bool> DeleteBasketAsync(string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId)) return false;

            await _basketRepository.ClearBasketAsync(buyerId);
            return true;
        }
    }
}
