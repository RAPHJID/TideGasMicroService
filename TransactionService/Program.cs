using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Services;
using TransactionService.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//AutoMapper
builder.Services.AddAutoMapper(typeof(Program));


// Add HttpClient support
builder.Services.AddHttpClient();

// Register typed clients
builder.Services.AddHttpClient("CustomerService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7261/api/Customer/");
});

builder.Services.AddHttpClient("CylinderService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7139/api/Cylinder/");
});


builder.Services.AddScoped<ITransactionService, TransactionsService>();

builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction Service API v1");
        c.RoutePrefix = "swagger";
    });
}




app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
