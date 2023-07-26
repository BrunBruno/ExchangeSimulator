using ExchangeSimulator.Api.Authorization;
using ExchangeSimulator.Application;
using ExchangeSimulator.Infrastructure;
using ExchangeSimulator.Infrastructure.Seeders;
using ExchangeSimulator.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddShared();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCustomAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// var scope = app.Services.CreateScope();
// var seeder = scope.ServiceProvider.GetRequiredService<GameSeeder>();
// await seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEndClient");

// app.UseHttpsRedirection();

app.UseShared();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }