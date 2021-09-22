using System;
using System.Threading.Tasks;

namespace Acesso.Account.Domain.Interfaces
{
    public interface ITransferTransactionApp
    {
        Model.TransferTransaction GetStatus(Guid id);
        Task<Model.TransferTransaction> GetStatusAsync(Guid id);
        Guid Transfer(string fromAccount, string toAccount, decimal value);
        Task<Guid> TransferAsync(string fromAccount, string toAccount, decimal value);
    }
}