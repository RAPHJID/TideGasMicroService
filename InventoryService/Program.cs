using System;
using System.Net.Http;
using InventoryService.Data;
using InventoryService.Profiles;
using InventoryService.Services;
using InventoryService.Services.IService;
using InventoryService.Services.HttpClients;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// === Database (SQL Server) ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Core Inventory Service ===
builder.Services.AddScoped<InventoryInterface, InventorysService>();

// === HttpClient for Cylinder Service ===
builder.Services.AddHttpClient<ICylinderHttpClient, CylinderHttpClient>((sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();

    // Prefer config from appsettings.json, fallback to localhost dev URL
    var baseUrl = cfg["CylinderApiBaseUrl"]
               ?? cfg["Services:CylinderBaseUrl"]
               ?? "https://localhost:7139/";

    client.BaseAddress = new Uri(baseUrl);
})
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        // ⚠️ DEV ONLY: Ignore SSL cert validation for localhost
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// === AutoMapper ===
builder.Services.AddAutoMapper(typeof(MappingProfile));

// === Controllers ===
builder.Services.AddControllers();

// === Swagger (avoid DTO naming conflicts) ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Inventory API",
        Version = "v1",
        Description = "Handles inventory and cylinder management"
    });
});

// === CORS ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// === Build the App ===
var app = builder.Build();

// === Middleware Pipeline ===
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
