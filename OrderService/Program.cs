using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.Data;
using OrderService.Profiles;
using OrderService.Services;
using OrderService.Services.HttpClients;
using OrderService.Services.IServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// === Database ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Dependency Injection ===
builder.Services.AddScoped<IOrdersService, OrdersService>();

// === AutoMapper ===
builder.Services.AddAutoMapper(typeof(MappingProfile));

// === Controllers ===
builder.Services.AddControllers();

// === Swagger ===
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

// ================== SERVICE URL SAFETY ==================

string RequireUrl(string key) =>
    builder.Configuration[key]
    ?? throw new Exception($"❌ Missing configuration: {key}");

builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(c =>
    c.BaseAddress = new Uri(RequireUrl("ServiceUrls:CustomerAPI")));

builder.Services.AddHttpClient<IInventoryApiClient, InventoryApiClient>(c =>
    c.BaseAddress = new Uri(RequireUrl("ServiceUrls:InventoryAPI")));

builder.Services.AddHttpClient<ITransactionApiClient, TransactionApiClient>(c =>
    c.BaseAddress = new Uri(RequireUrl("ServiceUrls:TransactionAPI")));

builder.Services.AddHttpClient<ICylinderApiClient, CylinderApiClient>(c =>
    c.BaseAddress = new Uri(RequireUrl("ServiceUrls:CylinderAPI")));

//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            )
        };
    });

var app = builder.Build();

// === Middleware ===
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactPolicy");//cors
app.UseAuthorization();
app.MapControllers();
app.Run();
