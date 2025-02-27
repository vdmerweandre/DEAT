# Double Entry Accounting and Treasury System (DEAT)

Simple proof of concept application to illustrate the usage of 2 different workflow technologies in [MassTransit with a saga state machine](https://masstransit.io/documentation/configuration/sagas/overview) and [Temporal](https://temporal.io/) in combination with a double entry accounting and treasury system.

## Overview
### Double Entry accounting
A double entry accounting system is often used in bookkeeping and represents the movement of funds between accounts. These fund movements are represented as debits and credits in a journal entry, each of which is recorded in an append only ledger. 

Double entry accounting system generally consists of
* Chart of accounts
* Journal entries to represent business transactions or the movement of funds to the accounts
* General append only Ledger showing each of the debits and credits 

#### Chart of Accounts
In and double entry accounting system, all movements of funds are represented as debits or credits to accounts in, one of the categories in the Chart of Accounts list below. 

A **chart of accounts (COA)** include all accounts categorised by either
* Assets (A)
* Expenses (E)
* Liabilities (L)
* Equity (C)
* Income (I)

In the poc, a default list of example accounts for each category were seeded, but more can be added.

#### Journal entries
A  business transaction (a journal entry) includes at least 1 x source and 1 x destination account and an amount such that the following equation always balances.
`Assets (A) + Expenses (E) = Liabilities (L) + Equity (C) + Income (I)`

* Debits represents money flowing into an account
* Credits represents money flowing out of an account

#### General Ledger
The General ledger is an append only account of all debits and credits to the accounts that form part of all the journal entries. It provides a read only account of what heppend during each leg of a journal entry and is often used to find irregularities or unespected balances in the accounts. 

#
### Workflows
For the purpose of this poc we will implement the following transaction workflow which are defined by the following states, events and commands:

The allowed **transaction states** are:
* `Created`
* `Approved`
* `Processed`
* `Completed`
* `Cancelled`
* `Failed`

These states can be changed by these **events/activities**
* `TransactionCreated`
* `TransactionSucceeded`
* `TransactionApproved`
* `TransactionProcessed`
* `TransactionLegConfirmed`
* `TransactionCancelled`
* `TransactionFailed`
* `TransactionRetried`
* `TransactionStatusRequested`

There are also these **commands/signals** that get fired from the statemachine
* `ApproveTransaction`
* `ProcessTransaction`
* `DebitAccount`
* `CreditAccount`
* `ConfirmTransaction`
* `ConfirmTransactionLeg`
* `CancelTransaction`
* `FailTransaction`
* `RetryTransaction`
* `UpdateTransactionStatus`

![Transaction state flow](/Images/Transaction_state_flow.jpeg)

### MassTransit and Saga State Machine
MassTransit is an open-source distributed application framework for .NET that provides a consistent abstraction on top of the supported message transports. The interfaces provided by MassTransit reduce message-based application complexity and allow developers to focus their effort on adding business value.

Sagas and State Machines is nothing more that a technology that keeps track of the state of a long running distributed task or transaction in providing reliable, durable, event-driven workflow orchestration

The poc uses an [in-memory state machine](https://masstransit.io/quick-starts/in-memory), but can be extended to persist the states to a datastore.

To get started with MassTransit, add MassTransit.Azure.ServiceBus.Core package to your project.

>$ dotnet add package MassTransit.Azure.ServiceBus.Core

### Temporal

Temporal delivers durable execution. It abstracts away the complexity of building scalable distributed systems and lets you keep focus on what matters - delivering reliable systems, faster.
It allows you to avoid coding for infrastructure nuances and their inevitable failures.

One way of thinking about temporal is a remote task manager where jobs get sent to and where their status and execution results are stored. 

Temporal is available in an self-hosted or cloud hosted solution. It is important to note that Temporal does not run your workflow or client workers, which you run and host yourself. Instead, it just seemlessly tracks the state of all stae, activity and events of your running workflow instances.

To setup a [local development environment](https://learn.temporal.io/getting_started/typescript/dev_environment/), download and start the local temporal server:

>$ temporal server start-dev

The Temporal Service will be available on `localhost:7233`.

The Temporal Web UI will be available at `http://localhost:8233`.

### Temporal vs. MassTransit Comparision
This document highlights the key differences and similarities between Temporal and MassTransit to help developers choose the right framework for their requirements.

| Feature/Aspect                  | Temporal                                                                 | MassTransit                                                               |
|---------------------------------|--------------------------------------------------------------------------|---------------------------------------------------------------------------|
| **Primary Focus**               | Workflow orchestration and stateful, long-running workflows             | Message-based communication and distributed system coordination            |
| **Programming Model**           | Workflow-centric with built-in state persistence and replay             | Message-driven with explicit state management (if required)                |
| **State Management**            | Automatically persisted and replayable workflows                        | Developers manage state explicitly (e.g., using sagas or external stores)  |
| **Trigger mechanism**           | Signals sent to the workflow                                            | Messages from external systems                                             |
| **Side Effects**                | Activities invoked by workflows                                         | Saga actions                                                               |
| **Observability**               | Built-in observability and tracing                                      | Requires custom logic                                                      |
| **Persistence**                 | Built-in event sourcing (via Temporal Server)                           | Depends on external persistence mechanisms (e.g., a database for sagas)    |
| **Retries and Compensation**    | Automatic retries and compensation built into workflows                 | Requires explicit configuration for retries and compensating logic         |
| **Timeout Handling**            | Built-in support for timeouts, delays, and timers in workflows          | Requires external mechanisms or manual coding for timeouts                 |
| **Language Support**            | Multi-language SDKs (e.g., Go, Java, Python, .NET, TypeScript)          | Primarily .NET (other languages via integrations like RabbitMQ)            |
| **Transport Layer**             | Uses Temporal Server for communication between client, worker, and server | Requires a message broker (e.g., RabbitMQ, Azure Service Bus, Kafka)     |
| **Scalability**                 | Horizontally scalable Temporal Server and workers                       | Scalable based on message broker and consumer configuration                |
| **Failure Recovery**            | Automatic workflow recovery from any failure point                      | Requires idempotent consumers and retry policies                           |
| **Event Ordering**              | Guarantees event and signal ordering in workflows                       | Depends on message broker and configuration (e.g., FIFO queues)            |
| **Integration with Ecosystem**  | Integrates with multiple languages and platforms                        | Integrates with .NET ecosystem and message brokers                         |
| **Learning Curve**              | Requires understanding of workflow concepts, Temporal server setup      | Easier for developers familiar with message-based patterns                 |
| **Debugging and Monitoring**    | Built-in Web UI and CLI for workflow history and monitoring             | Monitoring depends on external tools (e.g., RabbitMQ dashboards)           |
| **Use Cases**                   | Long-running workflows, stateful orchestration, retries, and scheduling | Event-driven architectures, sagas, and distributed communication           |
| **Deployment Complexity**       | Requires a Temporal Server setup (single point of management)           | Relies on message brokers and consumers (can be more complex to scale)     |
| **Community and Ecosystem**     | Growing community with extensive multi-language support                 | Mature .NET community and strong broker integrations                       |

## Conclusion

- Choose **Temporal** for complex workflows requiring state persistence, fault tolerance, and language flexibility.
- Choose **MassTransit** for event-driven systems and simpler messaging use cases, especially if you are working in the .NET ecosystem.

Both frameworks excel in specific scenarios, so the choice depends on your application requirements and team expertise.

## Getting Started

### Dependencies

* [.Net8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Masstransit](https://masstransit.io)
* [Temporal](https://temporal.io/)


### Executing program

* Open solution in Visual studio and run
* DEAT.WebAPI
* DEAT.AdminUI

### Referenecs
* [hexquote.com](https://hexquote.com/build-accounting-application-using-net-core-angular-and-entity-framework/)
* [Temporal Workflow Signals and Selectors](https://medium.com/lyonas/series-efficient-workflows-in-go-with-temporal-signals-selectors-ddd4bbc285d4#:~:text=Workflow%20Selectors%20in%20Temporal%20are,adaptability%20to%20the%20workflow%20execution.)