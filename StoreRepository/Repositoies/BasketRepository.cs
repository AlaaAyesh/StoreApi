using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using StoreCore.Entities;
using StoreCore.RepositoriesContract;

namespace StoreRepository.Repositoies
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer connectionMultiplexer) {
        
                _database= connectionMultiplexer.GetDatabase();
        }
        public async Task AddBasketAsync(CustomerBasket basket)
        {
            var created = await _database.StringSetAsync(basket.Id, System.Text.Json.JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));
            if (!created) {
                throw new Exception("Problem occur while saving the basket");
            }
        }
        public async Task ClearBasketAsync(string basketId)
        {
            var deleted = await _database.KeyDeleteAsync(basketId.ToString());
            if (!deleted) {
                throw new Exception("Problem occur while deleting the basket");
            }
        }


        public async Task<bool> BasketExistsAsync(int basketId)
        {
            return await _database.KeyExistsAsync(basketId.ToString());
        }
        public async Task<CustomerBasket> GetBasketByBuyerIdAsync(string buyerId)
        {
            var data = await _database.StringGetAsync(buyerId);
            if (data.IsNullOrEmpty) return null;
            return System.Text.Json.JsonSerializer.Deserialize<CustomerBasket>(data);
        }
        public async Task<CustomerBasket> GetBasketByIdAsync(int basketId)
        {
            var data = await _database.StringGetAsync(basketId.ToString());
            if (data.IsNullOrEmpty) return null;
            return System.Text.Json.JsonSerializer.Deserialize<CustomerBasket>(data);
        }
        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var updated = await _database.StringSetAsync(
                basket.Id,
                System.Text.Json.JsonSerializer.Serialize(basket),
                TimeSpan.FromDays(30)
            );

            if (!updated)
                throw new Exception("Problem occurred while updating the basket");

            // رجّع الـ basket بعد ما تحفظ
            return await GetBasketByBuyerIdAsync(basket.Id);
        }



    }
}
