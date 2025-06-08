using Application.ServicesImpl;
using Infrastructure.Services.Etherium;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {

            var rpcUrl = configuration.GetSection("Ethereum:RpcUrl").Value!;
            services.AddSingleton<IEthereumService>(new EthereumService(rpcUrl));

            return services;
        }
    }
}
