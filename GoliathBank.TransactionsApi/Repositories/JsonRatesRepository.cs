using System.Globalization;
using System.Text.Json;
using GoliathBank.TransactionsApi.Common;
using GoliathBank.TransactionsApi.Exceptions;
using GoliathBank.TransactionsApi.Models;

namespace GoliathBank.TransactionsApi.Repositories;

public class JsonRatesRepository : IRatesRepository
{
    private readonly ILogger<JsonRatesRepository> _logger;
    private readonly string _filePath;

    private IReadOnlyList<Rate>? _cache;

    public JsonRatesRepository(ILogger<JsonRatesRepository> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _filePath = Path.Combine(environment.ContentRootPath, "Data", "Rates.json");
    }


    public async Task<IReadOnlyList<Rate>> GetAllAsync()
    {
        if (_cache is not null) return _cache;
        
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);

            var raw = JsonSerializer.Deserialize<List<RateJson>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var rates = raw.Select(r => new Rate
                {
                    From = InputNormalization.RequiredUpper(r.from, "from"),
                    To = InputNormalization.RequiredUpper(r.to, "to"),
                    Value = ParseDecimalRequired(r.rate, "rate")
                })
                .ToList();
            _cache = rates;
            return _cache;
        }
        catch (DataFormatException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to read/parse rates.json");
            throw new DataFormatException("Invalid rates.json format.", e);
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
    
    private sealed class RateJson
    {
        public string? from { get; set; }
        public string? to { get; set; }
        public string? rate { get; set; }
    }
}