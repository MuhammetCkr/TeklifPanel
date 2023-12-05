using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Data.Abstract;

namespace TeklifPanel.Data.Concrete.EfCore
{
    public class EfCoreGenericRepository<TEntity> : IRepository<TEntity> where TEntity : class // Gelen TEntity'nin class olmak zorunda şeklinde bir kısıtlama
    {
        protected readonly DbContext _dbContext;
        public EfCoreGenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async void Create(TEntity entity)
        {
            try
            {
                await _dbContext.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<TEntity>> GetAll()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<ICollection<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbContext
                .Set<TEntity>()
                .Where(expression)
                .ToListAsync();
        }

        public TEntity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            try
            {

                return await _dbContext.Set<TEntity>().FindAsync(id);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAsync(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
            try
            {
                var result = await _dbContext.SaveChangesAsync();
                return result > 0 ? true : false;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ? true : false;

        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ? true : false;
        }
    }
}
