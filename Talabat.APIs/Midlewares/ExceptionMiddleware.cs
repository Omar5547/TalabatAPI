using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Midlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate Next ,ILogger<ExceptionMiddleware> logger, IHostEnvironment env )
        {
            _next = Next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context) 
        {
            try
            {
              await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ApiExceptionResponse response; // تعريف خارجي

                if (_env.IsDevelopment())
                {
                    response = new ApiExceptionResponse(
                        (int)HttpStatusCode.InternalServerError,
                        ex.Message,
                        ex.StackTrace?.ToString()
                    );
                }
                else
                {
                    response = new ApiExceptionResponse(
                        (int)HttpStatusCode.InternalServerError
                    );
                }
                var Options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                  
                };
                var JsonResponse = JsonSerializer.Serialize(response,Options);
                await context.Response.WriteAsync(JsonResponse);
            }


        }
    }

}
