using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain.Model
{
    public class AccountOperation
    {
        public string accountNumber { get; set; }
        public decimal value { get; set; }
        public string type { get; set; }
    }
}
