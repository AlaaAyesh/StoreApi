using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Entities;

namespace StoreCore.RepositoriesContract
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketByBuyerIdAsync(string buyerId);
        Task AddBasketAsync(CustomerBasket basket);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

        Task ClearBasketAsync(string basketId);
        Task<bool> BasketExistsAsync(int basketId);
    }
}
