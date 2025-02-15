using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace DEAT.WebAPI.Services
{
    public class JournalService : IJournalService
    {
        private readonly ILogger<JournalService> _logger;
        private readonly Dictionary<Guid, JournalEntry> _transactions = new();

        public JournalService(ILogger<JournalService> logger)
        {
            _logger = logger;

            //seed journal
            #region Simple deposit
            var id = Guid.Parse("5b0b940d-8f36-4eee-b57c-267fe737fe13");
            _transactions.Add(id,
                new JournalEntry
                {
                    TransactionId = id,
                    Reference = "Simple deposit",
                    State = "Success",
                    Timestamp = DateTime.UtcNow,
                    Version = 0,
                    JournalDetails = new JournalDetail[]
                    {
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("f96fa900-a4c6-4bf5-91a9-92eb6564fd87"),
                            AccountId = Guid.Parse("c4371215-948f-4d2f-8097-9312c53f9f21"),
                            Amount = 100,
                            Category = "Assets",
                            State = "",
                            Side = "Debit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("a13843a5-c882-416e-a004-16daa5e7b9d4"),
                            AccountId = Guid.Parse("7ccdffbd-c698-40e8-ba8b-09010beb0e85"),
                            Amount = 100,
                            Category = "Liabilities",
                            State = "",
                            Side = "Debit"
                        },
                    }
                });
            #endregion

            #region Deposit with fees
            id = Guid.Parse("519cb0b9-79ff-412d-ac6f-a8e8a0211998");
            _transactions.Add(id,
                new JournalEntry
                {
                    TransactionId = id,
                    Reference = "Deposit with fees",
                    State = "Success",
                    Timestamp = DateTime.UtcNow,
                    Version = 0,
                    JournalDetails = new JournalDetail[]
                    {
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("8955427c-997b-4354-bcce-5f99e2136b54"),
                            AccountId = Guid.Parse("c4371215-948f-4d2f-8097-9312c53f9f21"),
                            Amount = 100,
                            Category = "Assets",
                            State = "",
                            Side = "Debit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("ea388c24-6ff1-4144-b206-1ab134b052ae"),
                            AccountId = Guid.Parse("7ccdffbd-c698-40e8-ba8b-09010beb0e85"),
                            Amount = 98,
                            Category = "Liabilities",
                            State = "",
                            Side = "Debit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("864c910f-f241-43dc-8e01-e7fce57d9b92"),
                            AccountId = Guid.Parse("c2953603-2622-433e-a96c-39aba8fab744"),
                            Amount = 2,
                            Category = "Income",
                            State = "",
                            Side = "Debit"
                        },
                    }
                });
            #endregion

            #region Wrong - Deposit with fees + merchant fees
            id = Guid.Parse("3e87624f-6a42-41cd-ad69-12d270e153b6");
            _transactions.Add(id,
                new JournalEntry
                {
                    TransactionId = id,
                    Reference = "Deposit with fees + merchant fees",
                    State = "Success",
                    Timestamp = DateTime.UtcNow,
                    Version = 0,
                    JournalDetails = new JournalDetail[]
                    {
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("57375d2d-9a0f-423f-944b-9a3b2336bbe9"),
                            AccountId = Guid.Parse("c4371215-948f-4d2f-8097-9312c53f9f21"),
                            Amount = 100,
                            Category = "Assets",
                            State = "",
                            Side = "Debit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("27f4aa1b-4a2f-4e33-b407-e439b36a8dd8"),
                            AccountId = Guid.Parse("7ccdffbd-c698-40e8-ba8b-09010beb0e85"),
                            Amount = 98,
                            Category = "Liabilities",
                            State = "",
                            Side = "Debit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("9be8a2a7-a891-45cb-8921-692ac4166080"),
                            AccountId = Guid.Parse("c2953603-2622-433e-a96c-39aba8fab744"),
                            Amount = 2,
                            Category = "Income",
                            State = "",
                            Side = "Debit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("8e3275fb-9ece-4be9-b108-4635b4bb6097"),
                            AccountId = Guid.Parse("e10278dc-c66a-4c97-868b-2de6bf0bede2"),
                            Amount = 1,
                            Category = "Expenses",
                            State = "",
                            Side = "Debit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("57cf92d8-b95e-4cd0-9900-a7b008956941"),
                            AccountId = Guid.Parse("c2953603-2622-433e-a96c-39aba8fab744"),
                            Amount = 1,
                            Category = "Income",
                            State = "",
                            Side = "Credit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("8e3275fb-9ece-4be9-b108-4635b4bb6097"),
                            AccountId = Guid.Parse("e10278dc-c66a-4c97-868b-2de6bf0bede2"),
                            Amount = 1,
                            Category = "Expenses",
                            State = "",
                            Side = "Credit"
                        },
                    }
                });
            #endregion

            #region End of Month Merchant Fee settlement
            id = Guid.Parse("3685a4f3-cc41-4a09-8788-ced4a8c2e372");
            _transactions.Add(id,
                new JournalEntry
                {
                    TransactionId = id,
                    Reference = "End of Month Merchant Fee settlement",
                    State = "Success",
                    Timestamp = DateTime.UtcNow,
                    Version = 0,
                    JournalDetails = new JournalDetail[]
                    {
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("18113a2c-af38-4bc7-baf3-f48cd2f41bdc"),
                            AccountId = Guid.Parse("e10278dc-c66a-4c97-868b-2de6bf0bede2"),
                            Amount = 1,
                            Category = "Expenses",
                            State = "",
                            Side = "Credit"
                        },
                        new JournalDetail
                        {
                            TransactionLegId = Guid.Parse("32b499c4-6750-4b00-906d-39e1d4e8b1ff"),
                            AccountId = Guid.Parse("c2953603-2622-433e-a96c-39aba8fab744"),
                            Amount = 1,
                            Category = "Income",
                            State = "",
                            Side = "Credit"
                        },
                    }
                });
            #endregion
        }

        public Task<List<JournalEntry>> GetJournalEntriesAsync()
        {
            return Task.FromResult(_transactions.Values.ToList());
        }

        public async Task<Guid> CreateJournalEntryAsync(JournalEntry transaction)
        {
            if (_transactions.ContainsKey(transaction.TransactionId)) 
            {
                _logger.LogWarning($"Transaction already creates {transaction.TransactionId}");
                return await Task.FromResult(transaction.TransactionId);
            }

            // Generate a new Transaction Id
            transaction.TransactionId = Guid.NewGuid();

            _transactions.Add(transaction.TransactionId, transaction);

            return await Task.FromResult(transaction.TransactionId);
        }

        public async Task<Guid> UpdateJournalStateAsync(Guid transactionId, string state)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                _logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(transactionId);
            }

            _transactions[transactionId].State = state;
            foreach (var item in _transactions[transactionId].JournalDetails)
            {
                item.State = state;
            }

            return await Task.FromResult(transactionId);
        }

        public async Task<bool> ApproveTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                _logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].State = "Approved";
            foreach (var item in _transactions[transactionId].JournalDetails)
            {
                item.State = "Approved";
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> ConfirmTransactionLegAsync(Guid transactionId, Guid transactionLegId)
        {
            if (!_transactions.ContainsKey(transactionId) || !_transactions[transactionId].JournalDetails.Any(l => l.TransactionLegId == transactionLegId))
            {
                _logger.LogWarning($"Transaction or leg does not exists {transactionId} {transactionLegId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].JournalDetails.Single(l => l.TransactionLegId == transactionLegId).State = "Confirmed";

            return await Task.FromResult(true);// _transactions[transactionId].TransactionLegs.All(l => l.State == "Confirmed"));
        }

        public async Task<bool> CancelTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                _logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].State = "Cancelled";
            foreach (var item in _transactions[transactionId].JournalDetails)
            {
                item.State = "Cancelled";
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> RetryTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                _logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].State = "Created";
            foreach (var item in _transactions[transactionId].JournalDetails)
            {
                item.State = "";
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> ConfirmTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                _logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            return await Task.FromResult(_transactions[transactionId].JournalDetails.All(l => l.State == "Confirmed"));
        }
    }
}
