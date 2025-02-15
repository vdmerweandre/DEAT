# Double Entry Accounting and Treasury System (DEAT)

Simple proof of concept application to illustrate the usage of Masstransit and sagas in a double entry accounting and treasury system.

## Description
A double entry accounting system is often used in bookkeeping and represents the movement of funds between accounts. These fund movements are represented as debits and credits in a journal entry, each of which is recorded in an append only ledger. 

Double entry accounting system generally consists of
* Chart of accounts
* Journal entries to represent business transactions or the movement of funds to the accounts
* General append only Ledger showing each of the debits and credits 

### Chart of Accounts
In and double entry accounting system, all movements of funds are represented as debits or credits to accounts in, one of the categories in the Chart of Accounts list below. 

A **chart of accounts (COA)** include all accounts categorised by either
* Assets (A)
* Expenses (E)
* Liabilities (L)
* Equity (C)
* Income (I)

In the poc, a default list of example accounts for each category were seeded, but more can be added.

### Journal entries
A  business transaction (a journal entry) includes at least 1 x source and 1 x destination account and an amount such that the following equation always balances.
`Assets (A) + Expenses (E) = Liabilities (L) + Equity (C) + Income (I)`

* Debits represents money flowing into an account
* Credits represents money flowing out of an account

### General Ledger
The General ledger is an append only account of all debits and credits to the accounts that form part of all the journal entries. It provides a read only account of what heppend during each leg of a journal entry and is often used to find irregularities or unespected balances in the accounts. 

#
### Mass transit and sagas
A saga is nothing more that a technology that keeps track of the state of a long running distributed task or transction. A statemachine is a set of rules that governs any state transitions for all saga instances.

The poc uses an in-memory state machine, but can be extended to persist the states to a datastore.

The following states, events and commands are used for this example of processing transactions:

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

### Referenecs
* [hexquote.com](https://hexquote.com/build-accounting-application-using-net-core-angular-and-entity-framework/)