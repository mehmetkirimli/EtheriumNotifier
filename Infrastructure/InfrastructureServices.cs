using Application.Options;
using Application.ServicesImpl;
using Infrastructure.Services.Etherium;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EthereumOptions>(configuration.GetSection("Ethereum"));

            services.AddScoped<IEthereumService, EthereumService>();


            return services;
        }
    }
}
