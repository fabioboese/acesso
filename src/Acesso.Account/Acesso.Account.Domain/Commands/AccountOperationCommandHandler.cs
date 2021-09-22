using Acesso.Account.Domain.Interfaces;
using Acesso.Account.Domain.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Acesso.Account.Domain.Commands
{
    public class AccountOperationCommandHandler 
    {
        private IMessageBrokerConnection connection;
        private readonly Dictionary<TransferOperationCommand.OperationTypeEnum, TransferOperationStateMachine> successStateMachine;
        private readonly Dictionary<TransferOperationCommand.OperationTypeEnum, TransferOperationStateMachine> failureStateMachine;
        private readonly ITransferTransactionRepository repository;
        private readonly IOptions<ApplicationSettings> settings;
        private readonly ILogger logger;

        public AccountOperationCommandHandler(IMessageBrokerConnection connection, ILogger logger, ITransferTransactionRepository repository, IOptions<ApplicationSettings> settings)
        {
            this.connection = connection;
            this.logger = logger;
            this.repository = repository;
            this.settings = settings;
            successStateMachine = new Dictionary<TransferOperationCommand.OperationTypeEnum, TransferOperationStateMachine>();
            failureStateMachine = new Dictionary<TransferOperationCommand.OperationTypeEnum, TransferOperationStateMachine>();
            ConfigureStateMachine();
        }

        public void ConfigureStateMachine()
        {
            successStateMachine.Add(TransferOperationCommand.OperationTypeEnum.Withdraw, new TransferOperationStateMachine
            {
                NextOperationType = TransferOperationCommand.OperationTypeEnum.Deposit,
                NextTransactionStatus = Transaction.TransactionStatusEnum.Processing,
                Error = string.Empty,
                NextQueue = QueueConfiguration.ACCOUNT_OPERATION_DEPOSIT_QUEUE
            });
            successStateMachine.Add(TransferOperationCommand.OperationTypeEnum.Deposit, new TransferOperationStateMachine
            {
                NextOperationType = null,
                NextTransactionStatus = Transaction.TransactionStatusEnum.Confirmed,
                Error = string.Empty,
                NextQueue = null
            });
            successStateMachine.Add(TransferOperationCommand.OperationTypeEnum.Chargeback, new TransferOperationStateMachine
            {
                NextOperationType = null,
                NextTransactionStatus = Transaction.TransactionStatusEnum.Error,
                Error = "Estorno realizado com sucesso.",
                NextQueue = null
            });

            failureStateMachine.Add(TransferOperationCommand.OperationTypeEnum.Withdraw, new TransferOperationStateMachine
            {
                NextOperationType = null,
                NextTransactionStatus = Transaction.TransactionStatusEnum.Error,
                Error = "Não foi possível debitar o valor da conta origem.",
                NextQueue = null
            });
            failureStateMachine.Add(TransferOperationCommand.OperationTypeEnum.Deposit, new TransferOperationStateMachine
            {
                NextOperationType = TransferOperationCommand.OperationTypeEnum.Chargeback,
                NextTransactionStatus = Transaction.TransactionStatusEnum.Processing,
                Error = "Não foi possível creditar o valor na conta destino.",
                NextQueue = QueueConfiguration.ACCOUNT_OPERATION_DEPOSIT_QUEUE
            });
            failureStateMachine.Add(TransferOperationCommand.OperationTypeEnum.Chargeback, new TransferOperationStateMachine
            {
                NextOperationType = null,
                NextTransactionStatus = Transaction.TransactionStatusEnum.Error,
                Error = "Não foi possível realizar o estorno.",
                NextQueue = null
            });

        }

        public async Task HandleAsync(TransferOperationCommand message)
        {
            await repository.UpsertAsync(message.RefTransaction);
            var operation = new AccountOperation
            {
                accountNumber = message.OperationType == TransferOperationCommand.OperationTypeEnum.Deposit ? message.RefTransaction.DestinationAccountNumber : message.RefTransaction.SourceAccountNumber,
                value = message.RefTransaction.Amount,
                type = message.OperationType == TransferOperationCommand.OperationTypeEnum.Withdraw ? "Debit" : "Credit"
            };

            var result = await ExecuteAccountOperationAsync(operation);

            var stateMachine = (result.IsSuccessStatusCode ? successStateMachine : failureStateMachine)[message.OperationType];
            message.RefTransaction.Status = stateMachine.NextTransactionStatus;
            message.RefTransaction.errorMessage = result.ResponseContent;
            await repository.UpsertAsync(message.RefTransaction);

            if (stateMachine.NextOperationType.HasValue) message.OperationType = stateMachine.NextOperationType.Value;
            if (!string.IsNullOrEmpty(stateMachine.NextQueue)) EnqueueOperation(message, stateMachine.NextQueue);
        }


        private void EnqueueOperation(TransferOperationCommand command, string queueName)
        {
            var queue = connection.GetQueue<TransferOperationCommand>(queueName);
            queue.Publish(command);
        }

        private class AccountOperationResult
        {
            public bool IsSuccessStatusCode { get; set; }
            public string ResponseContent { get; set; }
        }

        private async Task<AccountOperationResult> ExecuteAccountOperationAsync(AccountOperation accountOperation)
        {
            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                var json = JsonConvert.SerializeObject(accountOperation);
                var uri = new Uri(settings.Value.AccountOperationApiRoute);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(uri, content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    logger.Log(responseContent);
                    return new AccountOperationResult { IsSuccessStatusCode = response.IsSuccessStatusCode, ResponseContent = responseContent };
                }
            }
        }
    }
}
