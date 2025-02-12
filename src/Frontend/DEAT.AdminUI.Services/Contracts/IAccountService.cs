using DEAT.Data.Models.Dtos;

namespace DEAT.AdminUI.Services.Contracts
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
        Task<Guid> CreateAccountAsync(AccountDto account);
    }
}
