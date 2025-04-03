using Microsoft.Extensions.DependencyInjection;
using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTigerBeetleServices(
        this IServiceCollection services,
        string[] clusterAddresses,
        UInt128 clusterID)
    {
        // Register the TigerBeetle client wrapper
        services.AddSingleton<ITigerBeetleClient>(sp => new TigerBeetleClientWrapper(clusterID, clusterAddresses));

        // Register the account registry as a singleton
        services.AddSingleton<IAccountRegistry, AccountRegistry>();

        // Register the account service
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}