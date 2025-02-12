using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine.Observers;
using MassTransit;

namespace DEAT.WebAPI.Services
{
    public class MessageService(
        IPublishEndpoint publishEndpoint) : IMessageService
    {
        public async Task RaiseEvent<T>(T message)
        {
            // Publish the StartTransaction event
            await publishEndpoint.Publish(message);
        }

        public async Task SendCommand<T>(T message)
        {
            // Publish the StartTransaction event
            await publishEndpoint.Publish(message);
        }
    }
}
