using GoliathBank.TransactionsApi.Dtos;

namespace GoliathBank.TransactionsApi.Services;

public interface ISkuService
{ 
    Task<SkuDetailResponse> GetSkuDetailAsync(string sku);
}