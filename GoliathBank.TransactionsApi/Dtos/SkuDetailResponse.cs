namespace GoliathBank.TransactionsApi.Dtos;

public class ConvertedTransaction
{
    public string Sku { get; set; } = "";
    public decimal OriginalAmount { get; set; }
    public string OriginalCurrency { get; set; } = "";
    public decimal EurAmount { get; set; }
}

public class SkuDetailResponse
{
    public string Sku { get; set; } = "";
    public List<ConvertedTransaction> Transactions { get; set; } = new();
    public decimal TotalEur { get; set; }
}
