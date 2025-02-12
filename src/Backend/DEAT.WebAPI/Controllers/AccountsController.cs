using Microsoft.AspNetCore.Mvc;
using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;

namespace DEAT.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController(
        IAccountService accountService,
        ILogger<AccountsController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetAllAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<AccountDto>> GetAllAccounts()
        {
            return await accountService.GetAllAccountsAsync();
        }

        [HttpPost(Name = "CreateAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Guid> CreateAccount(AccountDto account)
        {
            return await accountService.CreateAccountAsync(account);
        }
    }
}
