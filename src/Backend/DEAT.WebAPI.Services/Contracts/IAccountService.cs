
using DEAT.Data.Models.Dtos;

namespace DEAT.WebAPI.Services.Contracts
{
    public interface IAccountService
    {
        Task<List<Account>> GetAllAccountsAsync();
        Task<Guid> CreateAccountAsync(Account account);
        Task<bool> DebitAccountAsync(Guid accountId, decimal amount);
        Task<bool> CreditAccountAsync(Guid accountId, decimal amount);
    }
}
