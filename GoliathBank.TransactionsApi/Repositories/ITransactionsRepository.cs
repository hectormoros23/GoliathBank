using GoliathBank.TransactionsApi.Models;

namespace GoliathBank.TransactionsApi.Repositories;

public interface ITransactionsRepository
{
    Task<IReadOnlyList<Transaction>> GetAllAsync();
}