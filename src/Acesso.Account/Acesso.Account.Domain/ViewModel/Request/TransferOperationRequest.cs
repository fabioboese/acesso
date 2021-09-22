using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain.ViewModel.Request
{
    public class TransferOperationRequest
    {
        public string accountOrigin { get; set; }
        public string accountDestination { get; set; }
        public decimal value { get; set; }
    }
}
