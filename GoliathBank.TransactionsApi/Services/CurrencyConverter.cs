using System.Collections.Concurrent;
using GoliathBank.TransactionsApi.Exceptions;
using GoliathBank.TransactionsApi.Repositories;

namespace GoliathBank.TransactionsApi.Services;

public class CurrencyConverter : ICurrencyConverter
{
    private const string Target = "EUR";

    private readonly IRatesRepository _ratesRepository;
    private readonly ILogger<CurrencyConverter> _logger;
    
    private readonly ConcurrentDictionary<string, decimal> _toEurRateCache =
        new(StringComparer.OrdinalIgnoreCase);

    public CurrencyConverter(IRatesRepository ratesRepository, ILogger<CurrencyConverter> logger)
    {
        _ratesRepository = ratesRepository;
        _logger = logger;
    }

    public async Task<decimal> ConvertToEurAsync(string fromCurrency, decimal amount)
    {
        var from = Normalize(fromCurrency);
        if (from == "EUR") return amount;

        var rate = await GetToEurRateAsync(from);
        return amount * rate;
    }

    private async Task<decimal> GetToEurRateAsync(string from)
    {
        if (_toEurRateCache.TryGetValue(from, out var cached))
            return cached;

        var rates = await _ratesRepository.GetAllAsync();
        
        var graph = rates
            .GroupBy(r => r.From)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => (To: x.To, Rate: x.Value)).ToList(),
                StringComparer.OrdinalIgnoreCase
            );

        if (!graph.ContainsKey(from))
            throw new CurrencyConversionException(from, Target, "No outgoing rates for currency.");
        
        var q = new Queue<(string Curr, decimal AccRate)>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        q.Enqueue((from, 1m));
        visited.Add(from);

        while (q.Count > 0)
        {
            var (curr, acc) = q.Dequeue();
            if (!graph.TryGetValue(curr, out var edges)) continue;

            foreach (var (next, r) in edges)
            {
                if (r <= 0) continue;

                var nextAcc = acc * r;

                if (string.Equals(next, Target, StringComparison.OrdinalIgnoreCase))
                {
                    _toEurRateCache[from] = nextAcc;
                    return nextAcc;
                }

                if (visited.Add(next))
                    q.Enqueue((next, nextAcc));
            }
        }

        _logger.LogWarning("No conversion path found from {From} to EUR", from);
        throw new CurrencyConversionException(from, Target, "No conversion path found.");
    }

    private static string Normalize(string? s) => (s ?? "").Trim().ToUpperInvariant();

    private static decimal Round2(decimal v) =>
        Math.Round(v, 2, MidpointRounding.ToEven);
}