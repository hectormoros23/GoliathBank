namespace GoliathBank.TransactionsApi.Exceptions;

public class SkuNotFoundException : Exception
{
    public string Sku { get; }
    
    public SkuNotFoundException(string sku) : base($"Sku '{sku}' not found.")
    {
        Sku = sku;
    }
}