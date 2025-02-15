using DEAT.Data.Models.Dtos;

namespace DEAT.AdminUI.Services.Contracts
{
    public interface ILedgerService
    {
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesAsync();
    }
}
