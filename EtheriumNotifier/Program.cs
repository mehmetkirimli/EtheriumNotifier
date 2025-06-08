using Application;
using Domain;
using Infrastructure;
using Persistence;

var builder = WebApplication.CreateBuilder(args);


// Katman Servisleri
builder.Services.AddApplicationServices();
builder.Services.AddDomainServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();


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
