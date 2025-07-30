using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PoliMarket.Business.Contracts;
using PoliMarket.Business.Services;
using PoliMarket.DataAccess.Context;
using PoliMarket.DataAccess.Contracts;
using PoliMarket.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PoliMarket API",
        Version = "v1",
        Description = "API para el sistema PoliMarket - Reutilización de Software"
    });
});

// Database - Usar SQL Server
builder.Services.AddDbContext<PoliMarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository Pattern (Reutilización)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Business Services (Reutilización)
builder.Services.AddScoped<IComponenteCliente, ComponenteCliente>();
builder.Services.AddScoped<IComponenteProducto, ComponenteProducto>();
builder.Services.AddScoped<IComponenteStock, ComponenteStock>();
builder.Services.AddScoped<IComponenteVendedor, ComponenteVendedor>();
builder.Services.AddScoped<IComponenteVentas, ComponenteVentas>();

// CORS para frontends
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", builder =>
    {
        builder.WithOrigins("http://localhost:4200", "http://localhost:5173") // Angular y Vue
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PoliMarket API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseCors("AllowFrontends");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();