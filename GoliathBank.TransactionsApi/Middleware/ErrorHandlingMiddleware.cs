using System.Net;
using GoliathBank.TransactionsApi.Exceptions;

namespace GoliathBank.TransactionsApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (SkuNotFoundException ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await WriteJson(ctx, new { errorCode = "SKU_NOT_FOUND", message = ex.Message, sku = ex.Sku });
        }
        catch (CurrencyConversionException ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            await WriteJson(ctx, new { errorCode = "NO_CONVERSION_PATH", message = ex.Message, from = ex.From, to = ex.To });
        }
        catch (DataFormatException ex)
        {
            _logger.LogError(ex, "Data format error");
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteJson(ctx, new { errorCode = "INVALID_DATA_SOURCE", message = "Invalid data source format." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteJson(ctx, new { errorCode = "UNEXPECTED_ERROR", message = "Unexpected error." });
        }
    }

    private static async Task WriteJson(HttpContext ctx, object body)
    {
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(body);
    }
}