using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specification;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        #region Without Specification
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
                return await _dbContext.Set<T>().ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        #endregion


        #region With Specification
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }


        public async Task<T> GetEntityWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }
        #endregion
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvalutor<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        public async Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task Add(T item)
        {
             await _dbContext .Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        {
             _dbContext.Set<T>().Remove(item);
        }

        public void Update(T item)
        {
            _dbContext.Set<T>().Update(item);
        }
    }
}
