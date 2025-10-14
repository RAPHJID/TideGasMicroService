using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Data;
using OrderService.Profiles;
using OrderService.Services;
using OrderService.Services.IServices;
//using OrderService.Profiles;
//using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

// === Database ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Dependency Injection for Services ===
builder.Services.AddScoped<IOrdersService, OrdersService>();


// === Controllers ===
builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MappingProfile));



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
