using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DEAT.StateMachine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateMachineServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TransactionStateMachine, TransactionState>()
                    .InMemoryRepository();

                x.AddConsumer<DebitAccountConsumer>();
                x.AddConsumer<CreditAccountConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
