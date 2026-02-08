using GoliathBank.TransactionsApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GoliathBank.TransactionsApi.Controllers;

[ApiController]
[Route("rates")]
public class RatesController: ControllerBase
{
    private readonly IRatesRepository _ratesRepository;

    public RatesController(IRatesRepository ratesRepository) => _ratesRepository = ratesRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _ratesRepository.GetAllAsync());
    
}