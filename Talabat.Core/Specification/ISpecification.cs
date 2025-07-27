using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
    public interface ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T,bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        // prop for OrderBy [OrderBtDesc(P=>p.Name)]
        public Expression<Func<T, object>> OrderBy { get; set; }
        // prop for OrderByDescending [OrderByDesc(P=>p.Name)]
        public Expression<Func<T, object>> OrderByDescending { get; set; }

        //Take (2)
        public int Take { get; set; }

        public int Skip { get; set; }

        public bool IsPaginationEnabled { get; set; }
    }
}
