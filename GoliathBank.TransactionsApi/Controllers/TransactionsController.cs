using GoliathBank.TransactionsApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GoliathBank.TransactionsApi.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionsController: ControllerBase
{
    private readonly ITransactionsRepository _transactionsRepository;
    
    public TransactionsController(ITransactionsRepository transactionsRepository) 
        => _transactionsRepository = transactionsRepository;
    
    [HttpGet]
    public async Task<IActionResult> GetAll() 
        => Ok(await _transactionsRepository.GetAllAsync());
}