using Application.Options;
using Application.ServicesImpl;
using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure.Services.Etherium;
using Infrastructure.Services.HangFire;
using Infrastructure.Services.Notification;
using Infrastructure.Services.NotificationChannel;
using Infrastructure.Services.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Seed;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Options
            services.Configure<EthereumOptions>(configuration.GetSection("Ethereum"));
            services.Configure<NotificationOptions>(configuration.GetSection("Notification"));

            //Services
            services.AddScoped<IEthereumService, EthereumService>();
            services.AddScoped<INotificationChannelService, NotificationChannelService>();
            services.AddScoped<INotificationService,NotificationService>();
            services.AddScoped<ChannelSeeder>();

            // Redis için DI (Dependency Injection) 
            var redisHost = configuration["Redis:Host"];
            var redisPort = configuration["Redis:Port"];
            var redisConnection = $"{redisHost}:{redisPort}";
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
            services.AddSingleton<IRedisService, RedisService>();


            // Hangfire
            services.AddHangfire(config =>
            {
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();
                config.UseMemoryStorage();
            });
            services.AddHangfireServer();
            services.AddScoped<TransactionMonitorJob>();

            return services;
        }
    }
}
