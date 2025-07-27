using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Services;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services) 
        {
            Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            Services.AddScoped<IPaymentServices, PaymentService>();   
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped(typeof(IOrderServices), typeof(OrderServices));
            Services.AddScoped(typeof(IBasketRepository) , typeof(BasketRepository));
            //Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddAutoMapper(typeof(MappingProfiles));
            Services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory = (ActionContext) =>
                {
                    //ModelState => Dic [KeyValuePair]
                    //Key => Name of parameter
                    //Value => ErrorMessage
                    var errors = ActionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                    .SelectMany(p => p.Value.Errors)
                    .Select(E => E.ErrorMessage).ToArray();
                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });
            return Services;
        }
    }
}
