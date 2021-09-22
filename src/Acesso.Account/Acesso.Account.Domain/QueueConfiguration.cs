using Acesso.Account.Domain.Commands;
using Acesso.Account.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain
{
    public class QueueConfiguration
    {
        private readonly IMessageBrokerConnection connection;
        private readonly ILogger logger;
        private readonly ITransferTransactionRepository repository;
        private readonly IOptions<ApplicationSettings> settings;

        public QueueConfiguration(IMessageBrokerConnection connection, ILogger logger, ITransferTransactionRepository repository, IOptions<ApplicationSettings> settings)
        {
            this.connection = connection;
            this.logger = logger;
            this.repository = repository;
            this.settings = settings;
        }

        public const string ACCOUNT_OPERATION_DEPOSIT_QUEUE = "DEPOSITS";
        public const string ACCOUNT_OPERATION_WITHDRAWS_QUEUE = "WITHDRAWS";

        public void Configure()
        {
            //TODO: refazer isso
            var commandHandler = new AccountOperationCommandHandler(connection, logger, repository, settings);
            connection.GetQueue<TransferOperationCommand>(ACCOUNT_OPERATION_DEPOSIT_QUEUE).Subscribe(async (cmd) => await commandHandler.HandleAsync(cmd));
            connection.GetQueue<TransferOperationCommand>(ACCOUNT_OPERATION_WITHDRAWS_QUEUE).Subscribe(async (cmd) => await commandHandler.HandleAsync(cmd));
        }


    }
}
