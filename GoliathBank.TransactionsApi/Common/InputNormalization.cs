using GoliathBank.TransactionsApi.Exceptions;

namespace GoliathBank.TransactionsApi.Common;

public static class InputNormalization
{
    public static string RequiredUpper(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DataFormatException($"Missing or empty field '{fieldName}'.");

        return value.Trim().ToUpperInvariant();
    }
    
}