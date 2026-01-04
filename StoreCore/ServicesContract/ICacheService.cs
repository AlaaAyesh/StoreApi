using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCore.ServicesContract
{
    public interface ICacheService
    {
        Task  SetCacheKeyAsync(string key, object response, TimeSpan? expireTime = null);
        Task<string> GetCacheKeyAsync(string key);
    }
}
