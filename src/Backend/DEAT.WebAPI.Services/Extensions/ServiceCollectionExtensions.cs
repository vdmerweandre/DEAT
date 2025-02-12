using DEAT.WebAPI.Services.Consumers;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;
using DEAT.WebAPI.Services.Statemachine.Observers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DEAT.WebAPI.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDEATWebAPIServices(this IServiceCollection services,
            IConfiguration configuration,
            bool isProduction = true)
        {
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TransactionStateMachine, TransactionStateMachineInstance>()
                    .InMemoryRepository();

                x.AddConsumer<DebitAccountConsumer>();
                x.AddConsumer<CreditAccountConsumer>();
                x.AddConsumer<UpdateTransactionStatusConsumer>();
                x.AddConsumer<ConfirmTransactionConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);

                    // Connect the State Observer
                    var stateMachine = context.GetRequiredService<TransactionStateMachine>();
                    var stateObserver = context.GetRequiredService<AuditStateObserver<TransactionStateMachineInstance>>();
                    var eventObserver = context.GetRequiredService<AuditEventObserver<TransactionStateMachineInstance>>();
                    stateMachine.ConnectStateObserver(stateObserver);
                    stateMachine.ConnectEventObserver(eventObserver);
                });
            });

            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddScoped<IMessageService, MessageService>();

            services.AddSingleton<AuditStateObserver<TransactionStateMachineInstance>>();
            services.AddSingleton<AuditEventObserver<TransactionStateMachineInstance>>();

            services.AddSingleton<StateChangeLogService>();

            return services;
        }
    }
}
