using DEAT.Data.Models.Dtos;

namespace DEAT.WebAPI.Services.Contracts
{
    public interface IAccountService
    {
        Task<List<Account>> GetAllAccountsAsync();
        Task<System.UInt128?> CreateAccountAsync(Account account);
        Task<bool> DebitAccountAsync(System.UInt128? accountId, System.UInt128? amount);
        Task<bool> CreditAccountAsync(System.UInt128? accountId, System.UInt128? amount);
    }
}
