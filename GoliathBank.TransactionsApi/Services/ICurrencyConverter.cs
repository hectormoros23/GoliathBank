namespace GoliathBank.TransactionsApi.Services;

public interface ICurrencyConverter
{
    Task<decimal> ConvertToEurAsync(string fromCurrency, decimal amount);
}