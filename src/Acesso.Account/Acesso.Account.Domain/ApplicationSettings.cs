using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain
{
    public class ApplicationSettings
    {
        public string MessageBrokerConnectionString { get; set; }
        public string TransactionsDatabaseConnectionString { get; set; }
        public string LogLocation { get; set; }
        public string AccountOperationApiRoute { get; set; }
    }

}
