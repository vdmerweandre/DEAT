using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

public class TigerBeetleClientWrapper : ITigerBeetleClient
{
    private readonly Client _client;

    public TigerBeetleClientWrapper(UInt128 clusterId, string[] addresses)
    {
        _client = new Client(clusterId, addresses);
    }

    public Task<CreateAccountsResult[]> CreateAccountsAsync(Account[] accounts)
    {
        return _client.CreateAccountsAsync(accounts);
    }

    public Task<Account[]> LookupAccountsAsync(UInt128[] accountIds)
    {
        return _client.LookupAccountsAsync(accountIds);
    }

    public Task<CreateTransfersResult[]> CreateTransfersAsync(Transfer[] transfers)
    {
        return _client.CreateTransfersAsync(transfers);
    }
} 