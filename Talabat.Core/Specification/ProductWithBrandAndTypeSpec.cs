using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
    public class ProductWithBrandAndTypeSpec : BaseSpecifications<Product>
    {
        public ProductWithBrandAndTypeSpec(ProductSpecParams Params) 
            : base(P=> 
                ( string.IsNullOrEmpty(Params.Search)|| P.Name.ToLower().Contains (Params.Search))
                &&
                    (!Params.BrandId.HasValue || P.ProductBrandId == Params.BrandId)
                    && 
                    (!Params.TypeId.HasValue || P.ProductTypeId == Params.TypeId)
                   )
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);
            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch (Params.Sort) 
                {
                    case "PriceAsc":
                        AddOrderBy(P => P.Price);
                        break;
                        case "PriceDesc":
                        AddOrderByDescending(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }
            //Products =100
            //PageSize = 10
            //PageIndex = 5
            //Skip = 10 * (5 - 1) = 40
            //Take = 10
            ApplyPagination(Params.PageSize * (Params.PageIndex - 1), Params.PageSize);
        }

        ///Get Product By Id
        public ProductWithBrandAndTypeSpec(int id) :base(P=>P.Id == id)
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);
            
        }
    }
}
