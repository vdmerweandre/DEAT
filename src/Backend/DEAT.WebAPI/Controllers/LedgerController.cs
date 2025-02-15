using Microsoft.AspNetCore.Mvc;
using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;

namespace DEAT.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LedgerController(
        ILedgerService ledgerService,
        ILogger<LedgerController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetLegerEntries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<LedgerEntry>> GetLegerEntries()
        {
            return await ledgerService.GetLedgerAsync();
        }
    }
}
