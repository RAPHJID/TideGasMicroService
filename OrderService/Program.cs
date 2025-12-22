using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Data;
using OrderService.Profiles;
using OrderService.Services;
using OrderService.Services.HttpClients;
using OrderService.Services.IServices;



var builder = WebApplication.CreateBuilder(args);

// === Database ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Dependency Injection for Services ===
builder.Services.AddScoped<IOrdersService, OrdersService>();



//AUTOMAPPER
builder.Services.AddAutoMapper(typeof(MappingProfile));

// === Controllers ===
builder.Services.AddControllers();

// === Swagger (API Docs) ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });
    c.CustomSchemaIds(type => type.FullName);
});

// === CORS ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7261");
});

builder.Services.AddHttpClient<ICylinderApiClient, CylinderApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CylinderAPI"]);
});


// Typed HTTP client for communication with InventoryService
builder.Services.AddHttpClient<IInventoryApiClient, InventoryApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:InventoryAPI"]);
});

var app = builder.Build();

// === Middleware Pipeline ===
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
