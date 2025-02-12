using DEAT.Data.Models.Dtos;
using System.Collections.Concurrent;

namespace DEAT.AdminUI.Services.Contracts
{
    public interface IAuditService
    {
        Task<IEnumerable<StateChangeLog>> GetStateChangesAsync();
        Task<IEnumerable<EventLog>> GetEventLogsAsync();
    }
}
