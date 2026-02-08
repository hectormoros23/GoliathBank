using GoliathBank.TransactionsApi.Models;

namespace GoliathBank.TransactionsApi.Repositories;

public interface IRatesRepository
{
    Task<IReadOnlyList<Rate>> GetAllAsync();
}