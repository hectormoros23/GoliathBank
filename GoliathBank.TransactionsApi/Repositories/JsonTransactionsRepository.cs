using System.Globalization;
using System.Text.Json;
using GoliathBank.TransactionsApi.Common;
using GoliathBank.TransactionsApi.Exceptions;
using GoliathBank.TransactionsApi.Models;

namespace GoliathBank.TransactionsApi.Repositories;

public class JsonTransactionsRepository : ITransactionsRepository
{
    private readonly ILogger<JsonTransactionsRepository> _logger;
    private readonly string _filePath;

    private IReadOnlyList<Transaction>? _cache;

    public JsonTransactionsRepository(ILogger<JsonTransactionsRepository> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _filePath = Path.Combine(env.ContentRootPath, "Data", "transactions.json");
    }

    public async Task<IReadOnlyList<Transaction>> GetAllAsync()
    {
        if (_cache is not null) return _cache;

        try
        {
            var json = await File.ReadAllTextAsync(_filePath);

            var raw = JsonSerializer.Deserialize<List<TransactionJson>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var txs = raw.Select(t => new Transaction
                {
                    Sku = InputNormalization.RequiredUpper(t.sku, "sku"),
                    Currency = InputNormalization.RequiredUpper(t.currency, "currency"),
                    Amount = ParseDecimalRequired(t.amount, "amount")
                })
                .ToList();

            _cache = txs;
            return _cache;
        }
        catch (DataFormatException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read/parse transactions.json");
            throw new DataFormatException("Invalid transactions.json format.", ex);
        }
    }

    private static decimal ParseDecimalRequired(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DataFormatException($"Missing or empty field '{fieldName}'.");

        if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
            throw new DataFormatException($"Invalid decimal value for field '{fieldName}'.");

        return d;
    }

    private sealed class TransactionJson
    {
        public string? sku { get; set; }
        public string? amount { get; set; }
        public string? currency { get; set; }
    }
}