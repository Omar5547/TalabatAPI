using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Midlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
    public class Program
    {
        public  static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreContext>(options => 
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));        
            
            });
            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Connection);
            });

            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors(Options => 
            {
                Options.AddPolicy("MyPolicy" , options => 
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });


            var app = builder.Build();
            #region Update Database
            using var Scope = app.Services.CreateScope();
            var Services = Scope.ServiceProvider;
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>(); // لمعرفه الخطأ
            try 
            {
                var dbContext = Services.GetRequiredService<StoreContext>();
                await dbContext.Database.MigrateAsync();
                var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await IdentityDbContext.Database.MigrateAsync();
                var UserManger = Services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(UserManger); 
                await  StoreContextSeed.SeedAsync(dbContext);
            }
            catch (Exception ex) 
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "An error occurred during migration");
            }

            #endregion


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
               
                app.UseSwaggerMiddleware();
            }
            app.UseMiddleware<ExceptionMiddleware>();


            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();
          


            app.MapControllers();

            app.Run();
        }
    }
}
