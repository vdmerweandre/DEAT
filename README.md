# Double Entry Accounting and Treasury System (DEAT)

Simple proof of concept application to illustrate the usage of Masstransit and sagas in a double entry accounting and treasury system.

## Description

### Accounts
A **chart of accounts (COA)** include all accounts categorised by either
* Assets
* Liabilities
* Equity

A default list is seeded, but more can be added

#
### Transactions and transaction legs
Each transaction consist of a debit account, and amount to debit from that account, a list of transaction legs, each with a credit account and an amount to 
credit each of the tranasction leg accounts by. 

Each transaction have states governed by the Transaction state machine, which uses in-memory Masstrasit sagas. The implemented state flow is 
outlined in the state diagram below.

The allowed **transaction states** are:
* `Created`
* `Approved`
* `Processed`
* `Completed`
* `Cancelled`
* `Failed`

These states can be changed by these **events**
* `TransactionCreated`
* `TransactionSucceeded`
* `TransactionApproved`
* `TransactionProcessed`
* `TransactionLegConfirmed`
* `TransactionCancelled`
* `TransactionFailed`
* `TransactionRetried`
* `TransactionStatusRequested`

There are also these **internal command** that get fired form the statemachine
* `ProcessTransaction`
* `CreditAccount`
* `ConfirmTransaction`
* `UpdateTransactionStatus`

#

![Transaction state flow](/Images/Transaction_state_flow.jpeg)


## Getting Started

### Dependencies

* [.Net8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Masstransit](https://masstransit.io)


### Executing program

* Open solution in Visual studio and run
* DEAT.WebAPI
* DEAT.AdminUI
```