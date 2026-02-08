using GoliathBank.TransactionsApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GoliathBank.TransactionsApi.Controllers;

[ApiController]
[Route("Skus")]
public class SkusController : ControllerBase
{
    private readonly ISkuService _skuService;

    public SkusController(ISkuService skuService) => _skuService = skuService;
    
    
    [HttpGet("{sku}")]
    public async Task<IActionResult> GetSku(string sku)
        => Ok(await _skuService.GetSkuDetailAsync(sku));
    
}