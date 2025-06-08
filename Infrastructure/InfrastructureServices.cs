using Application.Options;
using Application.ServicesImpl;
using Infrastructure.Services.Etherium;
using Infrastructure.Services.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EthereumOptions>(configuration.GetSection("Ethereum"));

            services.AddScoped<IEthereumService, EthereumService>();

            // Redis 
            var redisHost = configuration["Redis:Host"];
            var redisPort = configuration["Redis:Port"];
            var redisConnection = $"{redisHost}:{redisPort}";

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

            services.AddSingleton<IRedisService, RedisService>();

            return services;
        }
    }
}
