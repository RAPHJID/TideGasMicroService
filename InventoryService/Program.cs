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

// === App DB for InventoryService ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Inventory domain service ===
builder.Services.AddScoped<InventoryInterface, InventorysService>();

// === Typed HttpClient to call CylinderService (server-to-server) ===
// The client is typed to ICylinderHttpClient (your wrapper implementation).
// Base URL read from configuration: "CylinderApiBaseUrl" or "Services:CylinderBaseUrl"
builder.Services.AddHttpClient<ICylinderHttpClient, CylinderHttpClient>((sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["CylinderApiBaseUrl"] ?? cfg["Services:CylinderBaseUrl"] ?? "https://localhost:7037/";
    client.BaseAddress = new Uri(baseUrl);
})
// DEV ONLY: accept local self-signed certs. Remove or change in production.
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// === MVC & AutoMapper ===
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// === Swagger with custom schema ids to avoid DTO name collisions ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // use full type name for Swagger schema ids to avoid collisions between services
    c.CustomSchemaIds(type => type.FullName);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });
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

var app = builder.Build();

// === Pipeline ===
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
