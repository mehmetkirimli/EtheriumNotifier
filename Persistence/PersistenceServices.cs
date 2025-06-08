using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DbContext;
using Persistence.Repositories;

namespace Persistence
{
    public static class PersistenceServices
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("EthereumDb"));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic repo


            return services;
        }
    }
}
