using DEAT.WebAPI.TigerBeetleServices.Collections;
using System.Collections.Concurrent;
using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

#nullable enable

public interface IAccountRegistry
{
    void AddAccount(UInt128 accountId, string? accountName = null);
    bool Contains(UInt128 accountId);
    IEnumerable<UInt128> GetAllAccountIds();
    string? GetAccountName(UInt128 accountId);
    void SetAccountName(UInt128 accountId, string accountName);
    IDictionary<UInt128, string> GetAllAccountNames();
}

public class AccountRegistry : IAccountRegistry
{
    private readonly ConcurrentHashSet<UInt128> _knownAccountIds = new();
    private readonly ConcurrentDictionary<UInt128, string> _accountNames = new();

    public void AddAccount(UInt128 accountId, string? accountName = null)
    {
        _knownAccountIds.Add(accountId);
        if (accountName != null)
        {
            _accountNames.TryAdd(accountId, accountName);
        }
    }

    public bool Contains(UInt128 accountId)
    {
        return _knownAccountIds.Contains(accountId);
    }

    public IEnumerable<UInt128> GetAllAccountIds()
    {
        return _knownAccountIds.ToArray();
    }

    public string? GetAccountName(UInt128 accountId)
    {
        return _accountNames.TryGetValue(accountId, out var name) ? name : null;
    }

    public void SetAccountName(UInt128 accountId, string accountName)
    {
        _accountNames[accountId] = accountName;
    }

    public IDictionary<UInt128, string> GetAllAccountNames()
    {
        return new Dictionary<UInt128, string>(_accountNames);
    }
}