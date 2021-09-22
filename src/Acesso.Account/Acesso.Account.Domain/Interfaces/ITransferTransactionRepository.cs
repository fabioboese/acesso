using Acesso.Account.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Acesso.Account.Domain.Interfaces
{
    public interface ITransferTransactionRepository
    {
        void Delete(Expression<Func<TransferTransaction, bool>> condition);
        void Delete(Guid id);
        Task DeleteAsync(Expression<Func<TransferTransaction, bool>> condition);
        Task DeleteAsync(Guid id);
        IEnumerable<TransferTransaction> Get(Expression<Func<TransferTransaction, bool>> expression);
        TransferTransaction Get(Guid id);
        Task<IEnumerable<TransferTransaction>> GetAsync(Expression<Func<TransferTransaction, bool>> expression);
        Task<TransferTransaction> GetAsync(Guid id);
        void Insert(IEnumerable<TransferTransaction> documents);
        void Insert(TransferTransaction document);
        Task InsertAsync(IEnumerable<TransferTransaction> documents);
        Task InsertAsync(TransferTransaction document);
        void Update(Expression<Func<TransferTransaction, bool>> expression, TransferTransaction document);
        void Update( TransferTransaction document);
        Task UpdateAsync(Expression<Func<TransferTransaction, bool>> expression, TransferTransaction document);
        Task UpdateAsync(TransferTransaction document);
        void Upsert(TransferTransaction document);
        Task UpsertAsync(TransferTransaction document);
    }
}