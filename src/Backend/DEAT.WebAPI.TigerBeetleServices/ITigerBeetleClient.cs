using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

public interface ITigerBeetleClient
{
    Task<CreateAccountsResult[]> CreateAccountsAsync(Account[] accounts);
    Task<Account[]> LookupAccountsAsync(UInt128[] accountIds);
    Task<CreateTransfersResult[]> CreateTransfersAsync(Transfer[] transfers);
}