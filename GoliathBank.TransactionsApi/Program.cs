using GoliathBank.TransactionsApi.Middleware;
using GoliathBank.TransactionsApi.Repositories;
using GoliathBank.TransactionsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddSingleton<IRatesRepository, JsonRatesRepository>();
builder.Services.AddSingleton<ITransactionsRepository, JsonTransactionsRepository>();
builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverter>();
builder.Services.AddSingleton<ISkuService, SkuService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

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