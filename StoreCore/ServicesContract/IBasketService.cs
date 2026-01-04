using StoreCore.Entities;
using System.Threading.Tasks;

namespace StoreCore.ServicesContract
{
    public interface IBasketService
    {
        Task<CustomerBasket?> GetBasketAsync(string buyerId);
        Task<CustomerBasket?> SaveOrUpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string buyerId);
    }
}
