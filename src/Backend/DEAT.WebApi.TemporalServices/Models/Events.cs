﻿
using DEAT.Data.Models.Dtos;

namespace DEAT.WebApi.TemporalServices.Models
{
    //Events - messages that can trigger state transitions. They must be correlated to a specific saga instance
    public record TransactionCreated(Guid TransactionId, JournalEntry JournalEntry);
    public record TransactionSucceeded(Guid TransactionId);
    public record TransactionApproved(Guid TransactionId);
    public record TransactionProcessed(Guid TransactionId);
    public record TransactionLegConfirmed(Guid TransactionId, Guid TransactionLegId);
    public record TransactionCancelled(Guid TransactionId);
    public record TransactionFailed(Guid TransactionId);
    public record TransactionRetried(Guid TransactionId);
}
