using System.Globalization;

namespace GoliathBank.TransactionsApiTest;

public class RoundingTests
{
    [Theory]
    [InlineData("1.005", "1.00")]
    [InlineData("1.015", "1.02")]
    [InlineData("2.505", "2.50")]
    [InlineData("2.515", "2.52")]
    [InlineData("-1.005", "-1.00")]
    
    public void BankersRounding_ToEven_Works(string input, string expected)
    {
        var dec = decimal.Parse(input, CultureInfo.InvariantCulture);
        var exp = decimal.Parse(expected, CultureInfo.InvariantCulture);

        var rounded = Math.Round(dec, 2, MidpointRounding.ToEven);

        Assert.Equal(exp, rounded);
    }
}