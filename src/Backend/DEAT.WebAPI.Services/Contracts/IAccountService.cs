
using DEAT.Data.Models.Dtos;

namespace DEAT.WebAPI.Services.Contracts
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAllAccountsAsync();
        Task<Guid> CreateAccountAsync(AccountDto account);
        Task<bool> DebitAccountAsync(Guid accountId, decimal amount);
        Task<bool> CreditAccountAsync(Guid accountId, decimal amount);
    }
}
