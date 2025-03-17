using Microsoft.AspNetCore.Mvc;
using DEAT.WebAPI.TigerBeetleServices;
using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.TigerBeetleServices.Exceptions;

namespace DEAT.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet(Name = "GetAllAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Account>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving accounts");
                return StatusCode(500, "An error occurred while retrieving accounts");
            }
        }

        [HttpPost(Name = "CreateAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Account>> CreateAccount(Account account)
        {
            try
            {
                var createdAccount = await _accountService.CreateAccountAsync(account);
                return Ok(createdAccount);
            }
            catch (AccountAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, "Attempt to create duplicate account");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account");
                return StatusCode(500, "An error occurred while creating the account");
            }
        }
    }
}
