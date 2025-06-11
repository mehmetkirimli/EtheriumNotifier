using Application;
using Application.Mapper;
using Domain;
using FluentValidation;
using Hangfire;
using Infrastructure;
using Infrastructure.Services.HangFire;
using Infrastructure.Services.Seed;
using Infrastructure.Validator;
using Persistence;
using Serilog;



var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var options = new WebApplicationOptions
{
    Args = args,
    EnvironmentName = environmentName,
    ContentRootPath = Directory.GetCurrentDirectory()
};
var builder = WebApplication.CreateBuilder(options);

// Ortam Yapýlandýrma
var environment = builder.Environment.EnvironmentName;
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" || environment == "Test")
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}
Console.WriteLine($"Loaded environment: {builder.Environment.EnvironmentName}");


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

// Existing code remains unchanged
builder.Services.AddValidatorsFromAssemblyContaining<CreateNotificationChannelRequestDtoValidator>(); // Ensure the correct type is passed here

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
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName=="Test")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Etherium Notifier API V1");
    });

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
