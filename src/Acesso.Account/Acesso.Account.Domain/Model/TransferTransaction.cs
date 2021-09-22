using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain.Model
{
    public class TransferTransaction : Transaction
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
