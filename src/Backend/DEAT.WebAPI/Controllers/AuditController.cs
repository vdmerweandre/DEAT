using Microsoft.AspNetCore.Mvc;
using DEAT.WebAPI.Services;
using DEAT.WebAPI.Services.Statemachine.Observers;
using DEAT.Data.Models.Dtos;
using System.Collections.Concurrent;

namespace DEAT.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly StateChangeLogService _stateObserver;

        public AuditController(StateChangeLogService stateObserver)
        {
            _stateObserver = stateObserver;
        }

        [HttpGet("states", Name = "GetStateChangeLogs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IReadOnlyList<StateChangeLog> GetStateChangeLogs()
        {
            return _stateObserver.GetStateChanges();
        }

        [HttpGet("events", Name = "GetEventLogs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<EventLog> GetEventLogs()
        {
            return _stateObserver.GetEventLogs();
        }
    }
}
