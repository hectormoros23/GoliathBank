namespace GoliathBank.TransactionsApi.Exceptions;

public class CurrencyConversionException : Exception
{
    public string From  { get; }
    public string To { get; }
    
    public CurrencyConversionException(string from, string to, string message) : base(message)
    { 
        From = from;
        To = to;
    }
}