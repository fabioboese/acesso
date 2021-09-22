using Acesso.Account.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain.Commands
{
    public class TransferOperationCommand
    {
        public OperationTypeEnum OperationType { get; set; }

        public TransferTransaction RefTransaction { get; set; }

        public enum OperationTypeEnum
        {
            Withdraw,
            Deposit,
            Chargeback
        }
    }
}
