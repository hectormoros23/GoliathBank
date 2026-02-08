using GoliathBank.TransactionsApi.Dtos;
using GoliathBank.TransactionsApi.Exceptions;
using GoliathBank.TransactionsApi.Repositories;
using GoliathBank.TransactionsApi.Services;

namespace GoliathBank.TransactionsApi.Services;

public class SkuService : ISkuService
{
    private readonly ITransactionsRepository _txRepo;
    private readonly ICurrencyConverter _converter;

    public SkuService(ITransactionsRepository txRepo, ICurrencyConverter converter)
    {
        _txRepo = txRepo;
        _converter = converter;
    }

    public async Task<SkuDetailResponse> GetSkuDetailAsync(string sku)
    {
        var normalizedSku = Normalize(sku);
        var all = await _txRepo.GetAllAsync();
        var txs = all.Where(t => string.Equals(t.Sku, normalizedSku, StringComparison.OrdinalIgnoreCase)).ToList();

        if (txs.Count == 0)
            throw new SkuNotFoundException(normalizedSku);

        var response = new SkuDetailResponse { Sku = normalizedSku };
        decimal total = 0m;
    
        foreach (var t in txs)
        {
            var eurPrecise = await _converter.ConvertToEurAsync(t.Currency, t.Amount);

            response.Transactions.Add(new ConvertedTransaction
            {
                Sku = t.Sku,
                OriginalAmount = t.Amount,
                OriginalCurrency = t.Currency,
                EurAmount = Math.Round(eurPrecise, 2, MidpointRounding.ToEven)
            });

            total += eurPrecise;
        }

        response.TotalEur = Math.Round(total, 2, MidpointRounding.ToEven);
        return response;
    }

    private static string Normalize(string? s) => (s ?? "").Trim().ToUpperInvariant();
}