using DEAT.Data.Models.Dtos;
using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

#nullable enable

public interface IAccountService
{
    Task<DEAT.Data.Models.Dtos.Account> CreateAccountAsync(DEAT.Data.Models.Dtos.Account account);

    Task<bool> CreditAccountAsync(UInt128? accountId, UInt128? amount);

    Task<bool> DebitAccountAsync(UInt128? accountId, UInt128? amount);

    Task<DEAT.Data.Models.Dtos.Account?> GetAccountAsync(UInt128? accountId);

    Task<IEnumerable<DEAT.Data.Models.Dtos.Account>> GetAllAccountsAsync();
}