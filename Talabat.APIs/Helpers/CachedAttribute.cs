using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expireTimeInSecond;

        public CachedAttribute(int ExpireTimeInSecond ) 
        {
            _expireTimeInSecond = ExpireTimeInSecond;
            
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var CacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var CacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
          var CachedRsponse =  await CacheService.GetCachedResponse(CacheKey);
            if (!string.IsNullOrEmpty(CachedRsponse)) 
            {
                var contextResult = new ContentResult()
                {
                    Content = CachedRsponse,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = contextResult;
                return;
                
            }
           var ExecutedEndPointContext=await next.Invoke();
            if (ExecutedEndPointContext.Result is OkObjectResult result) 
            {
               await CacheService.CacheResponseAsync(CacheKey, result.Value, TimeSpan.FromSeconds(_expireTimeInSecond));
            } 
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var KeyBuilder = new StringBuilder();
            KeyBuilder.Append(request.Path); //api //products
            foreach (var( key,value) in request.Query.OrderBy(X=>X.Key)) 
            {
                KeyBuilder.Append($"{ key}-{ value}");
            }
            return KeyBuilder.ToString();
        }
    }
}
