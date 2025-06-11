using Application;
using Application.Mapper;
using Domain;
using Hangfire;
using Infrastructure;
using Infrastructure.Services.HangFire;
using Infrastructure.Services.Seed;
using Persistence;
using Serilog;


var options = new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development,
    ContentRootPath = Directory.GetCurrentDirectory()
};
var builder = WebApplication.CreateBuilder(options);

builder.WebHost.UseUrls("http://0.0.0.0:80");

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

//AutoMapper
builder.Services.AddAutoMapper(typeof(Profiles).Assembly);

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
app.UseHangfireDashboard("/hangfire" , new DashboardOptions
{
    Authorization = new[] { new AllowAllDashboardAuthorizationFilter() } // Herkese izin ver
});
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
