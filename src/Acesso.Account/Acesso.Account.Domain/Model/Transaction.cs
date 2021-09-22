using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Domain.Model
{
    public abstract class Transaction
    {
        private TransactionStatusEnum status;

        public Guid id { get; set; }
        public DateTime startTime { get; set; } = DateTime.Now;
        public DateTime? endTime { get; set; } = null;
        public string errorMessage { get; set; }

        public TransactionStatusEnum Status
        {
            get { return status; }
            set
            {
                status = value;
                if (status == TransactionStatusEnum.Confirmed || status == TransactionStatusEnum.Error)
                    endTime = DateTime.Now;
            }
        }

        public enum TransactionStatusEnum
        {
            InQueue = 1,
            Processing = 2,
            Confirmed = 3,
            Error = 255
        }

    }
}
