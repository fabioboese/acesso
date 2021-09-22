using Acesso.Account.Domain;
using Acesso.Account.Domain.Commands;
using Acesso.Account.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Acesso.Account.Application
{
    public class TransferTransaction : ITransferTransactionApp
    {
        private IMessageBrokerConnection queueConnection;
        private ITransferTransactionRepository repository;

        public TransferTransaction(IMessageBrokerConnection queueConnection, ITransferTransactionRepository repository)
        {
            this.queueConnection = queueConnection;
            this.repository = repository;
        }

        private TransferOperationCommand GetTransferOperationCommand(string fromAccount, string toAccount, decimal value)
        {
            var transfer = new Domain.Model.TransferTransaction
            {
                id = Guid.NewGuid(),
                SourceAccountNumber = fromAccount,
                DestinationAccountNumber = toAccount,
                Amount = value
            };

            var message = new TransferOperationCommand
            {
                OperationType = TransferOperationCommand.OperationTypeEnum.Withdraw,
                RefTransaction = transfer
            };

            return message;
        }

        public Domain.Model.TransferTransaction GetStatus(Guid id)
        {
            var transfer = repository.Get(id);
            return transfer;
        }

        public async Task<Domain.Model.TransferTransaction> GetStatusAsync(Guid id)
        {
            var transfer = await repository.GetAsync(id);
            return transfer;
        }

        public Guid Transfer(string fromAccount, string toAccount, decimal value)
        {
            var message = GetTransferOperationCommand(fromAccount, toAccount, value);
            queueConnection.GetQueue<TransferOperationCommand>(QueueConfiguration.ACCOUNT_OPERATION_WITHDRAWS_QUEUE).Publish(message);
            return message.RefTransaction.id;
        }


        public async Task<Guid> TransferAsync(string fromAccount, string toAccount, decimal value)
        {
            var message = GetTransferOperationCommand(fromAccount, toAccount, value);
            await Task.Run(() => queueConnection.GetQueue<TransferOperationCommand>(QueueConfiguration.ACCOUNT_OPERATION_WITHDRAWS_QUEUE).Publish(message));
            return message.RefTransaction.id;
        }
    }
}
