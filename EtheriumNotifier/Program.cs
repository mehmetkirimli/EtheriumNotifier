using Application;
using Domain;
using Hangfire;
using Infrastructure;
using Infrastructure.Services.HangFire;
using Persistence;
using Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Katman Servisleri  
builder.Services.AddApplicationServices();
builder.Services.AddDomainServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Serilog  
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
);

// Controller  
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI  
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Etherium Notifier API",
        Version = "v1",
        Description = " Ethereum Transactions API for Notification "
    });
});

var app = builder.Build();

#region Seeder Baþlangýçta Çalýþsýn Örnek NotificationChannel oluþsun
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ChannelSeeder>();
    await seeder.SeedAsync();
}
#endregion

// Hangfire  
app.UseHangfireDashboard("/hangfire");
HangfireJobRegistration.RegisterJobs();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
