using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Repositories
{
        
    public interface IRepository<T> where T : class
    {
      Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
      Task<T> GetByIdAsync(int id);
      Task AddAsync(T entity);
      Task UpdateAsync(T entity);
      Task DeleteAsync(int id);
      Task DeleteAsync(Guid id);
      Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> filter);
      Task<bool> ExistsAsync(string hash);
      Task<List<T>> GetAllAsync();
      Task SaveChangesAsync();
    }
}
