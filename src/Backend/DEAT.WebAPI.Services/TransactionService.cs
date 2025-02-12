using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace DEAT.WebAPI.Services
{
    public class TransactionService(ILogger<TransactionService> logger) : ITransactionService
    {
        private readonly Dictionary<Guid, TransactionDto> _transactions = new();

        public Task<List<TransactionDto>> GetTransactionsAsync()
        {
            return Task.FromResult(_transactions.Values.ToList());
        }

        public async Task<Guid> CreateTransactionAsync(TransactionDto transaction)
        {
            if (_transactions.ContainsKey(transaction.TransactionId)) 
            {
                logger.LogWarning($"Transaction already creates {transaction.TransactionId}");
                return await Task.FromResult(transaction.TransactionId);
            }

            // Generate a new Transaction Id
            transaction.TransactionId = Guid.NewGuid();

            _transactions.Add(transaction.TransactionId, transaction);

            return await Task.FromResult(transaction.TransactionId);
        }

        public async Task<Guid> UpdateTransactionStateAsync(Guid transactionId, string state)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(transactionId);
            }

            _transactions[transactionId].Status = state;
            foreach (var item in _transactions[transactionId].TransactionLegs)
            {
                item.State = state;
            }

            return await Task.FromResult(transactionId);
        }

        public async Task<bool> ApproveTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].Status = "Approved";
            foreach (var item in _transactions[transactionId].TransactionLegs)
            {
                item.State = "Approved";
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> ConfirmTransactionLegAsync(Guid transactionId, Guid transactionLegId)
        {
            if (!_transactions.ContainsKey(transactionId) || !_transactions[transactionId].TransactionLegs.Any(l => l.TransactionLegId == transactionLegId))
            {
                logger.LogWarning($"Transaction or leg does not exists {transactionId} {transactionLegId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].TransactionLegs.Single(l => l.TransactionLegId == transactionLegId).State = "Confirmed";

            return await Task.FromResult(true);// _transactions[transactionId].TransactionLegs.All(l => l.State == "Confirmed"));
        }

        public async Task<bool> CancelTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].Status = "Cancelled";
            foreach (var item in _transactions[transactionId].TransactionLegs)
            {
                item.State = "Cancelled";
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> RetryTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            _transactions[transactionId].Status = "Created";
            foreach (var item in _transactions[transactionId].TransactionLegs)
            {
                item.State = "";
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> ConfirmTransactionAsync(Guid transactionId)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                logger.LogWarning($"Transaction does not exists {transactionId}");
                return await Task.FromResult(false);
            }

            return await Task.FromResult(_transactions[transactionId].TransactionLegs.All(l => l.State == "Confirmed"));
        }
    }
}
