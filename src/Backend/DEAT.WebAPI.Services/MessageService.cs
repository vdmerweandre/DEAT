using DEAT.WebAPI.Services.Contracts;
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
