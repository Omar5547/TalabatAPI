using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext dbContext)
        {
            if (!dbContext.ProductBrands.Any())
            {
                var BrandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);
                if (Brands?.Count > 0)
                {
                    foreach (var brand in Brands)
                    {
                        await dbContext.Set<ProductBrand>().AddAsync(brand);

                    }
                   
                }
                await dbContext.SaveChangesAsync();
            }
            // Seed Product Types
            if (!dbContext.ProductTypes.Any())
            {
                var TypesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json");
                 var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);
                 if (Types?.Count > 0) 
                 {
                     foreach (var type in Types)
                     {
                        await dbContext.Set<ProductType>().AddAsync(type);
                     }
                       
                 }
                await dbContext.SaveChangesAsync();
            }

            // Seed ProductsData
            if (!dbContext.Products.Any())
            {
                var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                if (Products?.Count > 0)
                {
                    foreach (var product in Products)
                    {
                        await dbContext.Set<Product>().AddAsync(product);
                    }
                   
                }
                await dbContext.SaveChangesAsync();
            }
            if (!dbContext.DeliveryMethods.Any())
            {
                var DeliveryMethodsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
                var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
                if (DeliveryMethods?.Count > 0)
                {
                    foreach (var DeliveryMethod in DeliveryMethods)
                    {
                        await dbContext.Set<DeliveryMethod>().AddAsync(DeliveryMethod);
                    }

                }
                await dbContext.SaveChangesAsync();
            }
           
        }

    }
}

