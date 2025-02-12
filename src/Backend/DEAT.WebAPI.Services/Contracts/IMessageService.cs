using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEAT.WebAPI.Services.Contracts
{
    public interface IMessageService
    {
        Task SendCommand<T>(T message);
        Task RaiseEvent<T>(T message);
    }
}
