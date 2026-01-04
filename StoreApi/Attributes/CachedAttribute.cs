using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using StoreCore.ServicesContract;

namespace StoreApis.Attributes
{
    public class CachedAttribute : Attribute ,IAsyncActionFilter
    {
        private readonly int expireTime;

        public CachedAttribute(int expireTime )
        {
            this.expireTime = expireTime;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var cacheService= context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = GenerateCachKeyFromRequest(context.HttpContext.Request);
            var cacheResponse = await cacheService.GetCacheKeyAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cacheResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;
                return;
            }

            var executed = await next();
            if(executed.Result is OkObjectResult result)
            {
                await cacheService.SetCacheKeyAsync(cacheKey, result.Value, TimeSpan.FromSeconds(expireTime));

            }

        }

        private string GenerateCachKeyFromRequest(HttpRequest request)
        {
            var cacheKey = new StringBuilder();
            cacheKey.Append($"{request.Path}");
            foreach(var (key , value)in request.Query.OrderBy(x => x.Key))
            {
                cacheKey.Append($"|{key}-{value}");

            }
            return cacheKey.ToString();
        }
    }
}
