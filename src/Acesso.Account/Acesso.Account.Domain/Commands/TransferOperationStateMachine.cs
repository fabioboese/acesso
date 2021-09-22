using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain.Commands
{
    public class TransferOperationStateMachine
    {
        public TransferOperationCommand.OperationTypeEnum? NextOperationType { get; set; }
        public Model.Transaction.TransactionStatusEnum NextTransactionStatus { get; set; }
        public string Error { get; set; }
        public string NextQueue { get; set; }
    }
}
