using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specification;

namespace Talabat.Repository
{
    public static class SpecificationEvalutor<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec) 
        {
            var Query = inputQuery; //_dbContext.Set<T>();
            if ( spec.Criteria is not null) //p => p.Id = = id
            {
              Query = Query.Where(spec.Criteria);//_dbContext.Set<T>().Where(P => p.Id == id);
            }
            if (spec.OrderBy is not null) //p => p.Name
            {
                Query = Query.OrderBy(spec.OrderBy); // _dbContext.Set<T>().OrderBy(P => P.Name);
            }
            if (spec.OrderByDescending is not null) //p => p.Name
            {
                Query = Query.OrderByDescending(spec.OrderByDescending); // _dbContext.Set<T>().OrderByDescending(P => P.Name);
            }
            if (spec.IsPaginationEnabled)
            {
                Query = Query.Skip(spec.Skip).Take(spec.Take);
            }
            //p => p.ProductBrand , p => p.ProductType
            Query = spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
            return Query;
        }
    }
}
