using System.Net.Http;
using InventoryService.Data;
using InventoryService.Profiles;
using InventoryService.Services;
using InventoryService.Services.IService;
using InventoryService.Services.HttpClients; // <— add this namespace (where ICylinderHttpClient & CylinderHttpClient live)
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === DB for InventoryService ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Inventory domain services ===
builder.Services.AddScoped<InventoryInterface, InventorysService>();

// === Call CylinderService via HttpClient (no direct DI of CylindersService here) ===
// Base URL from config: "CylinderApiBaseUrl": "https://localhost:7037/"
builder.Services.AddHttpClient("CylinderApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CylinderApiBaseUrl"] ?? "https://localhost:7037/");
})
// DEV ONLY: accept local self-signed certs. Remove this in prod.
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// Register the wrapper
builder.Services.AddScoped<ICylinderHttpClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("CylinderApi");
    return new CylinderHttpClient(client);
});

// === MVC & extras ===
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// === CORS (browser -> InventoryService only; server-to-server calls ignore CORS) ===
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
