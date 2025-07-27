using System.Runtime.CompilerServices;

namespace Talabat.APIs.Extensions
{
    public static class AddSwaggerExtensions
    {
        public static WebApplication UseSwaggerMiddleware(this WebApplication app) 
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
