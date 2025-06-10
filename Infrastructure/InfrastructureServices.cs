using Application.Options;
using Application.ServicesImpl;
using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure.Services.Etherium;
using Infrastructure.Services.HangFire;
using Infrastructure.Services.Notification;
using Infrastructure.Services.NotificationChannel;
using Infrastructure.Services.Redis;
using Infrastructure.Services.Seed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IWeb3Factory, Web3Factory>();
            services.AddScoped<ChannelSeeder>();

            // Redis için DI (Dependency Injection) 

            var redisConfig = configuration.GetSection("Redis");

            var options = new ConfigurationOptions
            {
                EndPoints = { $"{redisConfig["Host"]}:{redisConfig.GetValue<int>("Port")}" },
                ConnectTimeout = redisConfig.GetValue<int>("ConnectTimeout", 15000),
                SyncTimeout = redisConfig.GetValue<int>("SyncTimeout", 15000),
                AbortOnConnectFail = false
            };

            var redis = ConnectionMultiplexer.Connect(options);
            services.AddSingleton<IConnectionMultiplexer>(redis);
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
