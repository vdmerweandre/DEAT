using DEAT.Data.Models.Dtos;

namespace DEAT.AdminUI.Services.Contracts
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Guid> CreateAccountAsync(Account account);
    }
}
