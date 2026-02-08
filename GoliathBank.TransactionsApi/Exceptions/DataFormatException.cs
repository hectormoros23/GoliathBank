namespace GoliathBank.TransactionsApi.Exceptions;

public class DataFormatException : Exception
{
    public DataFormatException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}