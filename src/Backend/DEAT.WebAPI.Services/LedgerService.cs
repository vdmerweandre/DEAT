using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace DEAT.WebAPI.Services
{
    public class LedgerService : ILedgerService
    {
        private readonly ILogger<LedgerService> _logger;
        private readonly List<LedgerEntry> _ledger = new();

        public LedgerService(ILogger<LedgerService> logger)
        {
            _logger = logger;

            //seed ledger
            #region Simple deposit
            _ledger.Add(new LedgerEntry { 
                TransactionId = Guid.Parse("5b0b940d-8f36-4eee-b57c-267fe737fe13"),
                TransactionLegId = Guid.Parse("f96fa900-a4c6-4bf5-91a9-92eb6564fd87"),
                AccountId = Guid.Parse("c4371215-948f-4d2f-8097-9312c53f9f21"),
                Amount = 100,
                
                Side = "Debit",
                Timestamp = DateTime.UtcNow,
            });
            _ledger.Add(new LedgerEntry
            {
                TransactionId = Guid.Parse("5b0b940d-8f36-4eee-b57c-267fe737fe13"),
                TransactionLegId = Guid.Parse("a13843a5-c882-416e-a004-16daa5e7b9d4"),
                AccountId = Guid.Parse("7ccdffbd-c698-40e8-ba8b-09010beb0e85"),
                Amount = 100,
                Side = "Debit",
                Timestamp = DateTime.UtcNow,
            });
            #endregion

            #region Deposit with fees 
            _ledger.Add(new LedgerEntry
            {
                TransactionId = Guid.Parse("519cb0b9-79ff-412d-ac6f-a8e8a0211998"),
                TransactionLegId = Guid.Parse("8955427c-997b-4354-bcce-5f99e2136b54"),
                AccountId = Guid.Parse("c4371215-948f-4d2f-8097-9312c53f9f21"),
                Amount = 100,
                Side = "Debit",
                Timestamp = DateTime.UtcNow,
            });
            _ledger.Add(new LedgerEntry
            {
                TransactionId = Guid.Parse("519cb0b9-79ff-412d-ac6f-a8e8a0211998"),
                TransactionLegId = Guid.Parse("ea388c24-6ff1-4144-b206-1ab134b052ae"),
                AccountId = Guid.Parse("7ccdffbd-c698-40e8-ba8b-09010beb0e85"),
                Amount = 98,
                Side = "Debit",
                Timestamp = DateTime.UtcNow,
            });
            _ledger.Add(new LedgerEntry
            {
                TransactionId = Guid.Parse("519cb0b9-79ff-412d-ac6f-a8e8a0211998"),
                TransactionLegId = Guid.Parse("864c910f-f241-43dc-8e01-e7fce57d9b92"),
                AccountId = Guid.Parse("c2953603-2622-433e-a96c-39aba8fab744"),
                Amount = 2,
                Side = "Debit",
                Timestamp = DateTime.UtcNow,
            });
            #endregion

            #region End of Month Merchant Fee settlement
            _ledger.Add(new LedgerEntry
            {
                TransactionId = Guid.Parse("3685a4f3-cc41-4a09-8788-ced4a8c2e372"),
                TransactionLegId = Guid.Parse("18113a2c-af38-4bc7-baf3-f48cd2f41bdc"),
                AccountId = Guid.Parse("e10278dc-c66a-4c97-868b-2de6bf0bede2"),
                Amount = 1,
                Side = "Credit",
                Timestamp = DateTime.UtcNow,
            });
            _ledger.Add(new LedgerEntry
            {
                TransactionId = Guid.Parse("3685a4f3-cc41-4a09-8788-ced4a8c2e372"),
                TransactionLegId = Guid.Parse("32b499c4-6750-4b00-906d-39e1d4e8b1ff"),
                AccountId = Guid.Parse("c2953603-2622-433e-a96c-39aba8fab744"),
                Amount = 1,
                Side = "Credit",
                Timestamp = DateTime.UtcNow,
            });
            #endregion
        }

        public Task<List<LedgerEntry>> GetLedgerAsync()
        {
            return Task.FromResult(_ledger);
        }

        public Task AppendAsync(LedgerEntry legerEntry)
        {
            _ledger.Add(legerEntry);

            return Task.CompletedTask;
        }
    }
}
