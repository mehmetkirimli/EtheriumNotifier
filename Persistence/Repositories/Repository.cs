using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContext;
using Serilog;

namespace Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _dbSet.Where(filter).ToListAsync();
            }
            else
            {
                return await _dbSet.ToListAsync();
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                Log.Warning($"Entity with ID {id} not found in the database."); // Burada log yazılıyor
                return null; // Eğer entity bulunamazsa null döndürüyoruz
            }
            return entity;
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            Log.Information($"Entity of type {typeof(T).Name} added successfully.");
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            Log.Information($"Entity of type {typeof(T).Name} updated successfully.");
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                Log.Warning($"Entity with ID {id} not found in the database for deletion.");
                return;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            Log.Information($"Entity with ID {id} deleted successfully.");
        }

        //Overloaded metod Delete Id için var ama Guid için ekledim
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                Log.Warning($"Entity with ID {id} not found in the database for deletion.");
                return;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            Log.Information($"Entity with ID {id} deleted successfully.");
        }

        public async Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync(); //TODO Bu metot ile GetAll metodu birleşitirilebilir aslında. Aynısı nerdeyse
        }

        public async Task<bool> ExistsAsync(string hash)
        {
            return await _dbSet.AnyAsync(e => EF.Property<string>(e, "Hash") == hash); // "Hash" özelliği var mı kontrol ediyoruz
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync(); // Tüm verileri liste olarak döndürür
        }

      
    }
}
