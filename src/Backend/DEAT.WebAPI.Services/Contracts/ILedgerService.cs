
using DEAT.Data.Models.Dtos;

namespace DEAT.WebAPI.Services.Contracts
{
    public interface ILedgerService
    {
        Task<List<LedgerEntry>> GetLedgerAsync();
        Task AppendAsync(LedgerEntry legerEntry);
    }
}
