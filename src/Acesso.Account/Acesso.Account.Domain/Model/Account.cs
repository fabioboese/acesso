using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain.Model
{
    public class Account
    {
        public int id { get; set; }
        public string accountNumber { get; set; }
        public decimal balance { get; set; }
    }
}
