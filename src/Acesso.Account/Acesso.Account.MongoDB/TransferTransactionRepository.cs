using Acesso.Account.Domain;
using Acesso.Account.Domain.Interfaces;
using Acesso.Account.Domain.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Acesso.Account.MongoDB
{
    public class TransferTransactionRepository : ITransferTransactionRepository
    {
        private IMongoCollection<TransferTransaction> collection;

        public TransferTransactionRepository(IOptions<ApplicationSettings> applicationSettings)
        {
            var client = new MongoClient(applicationSettings.Value.TransactionsDatabaseConnectionString);
            var database = client.GetDatabase("acessoDb");
            collection = database.GetCollection<TransferTransaction>("transactions");
        }

        public TransferTransaction Get(Guid id)
        {
            var find = collection.Find(x => x.id == id);
            return find.FirstOrDefault();
        }

        public async Task<TransferTransaction> GetAsync(Guid id)
        {
            var find = await collection.FindAsync(x => x.id == id);
            return find.FirstOrDefault();
        }

        public IEnumerable<TransferTransaction> Get(Expression<Func<TransferTransaction, bool>> expression)
        {
            var find = collection.Find(expression);
            return find.ToEnumerable();
        }

        public async Task<IEnumerable<TransferTransaction>> GetAsync(Expression<Func<TransferTransaction, bool>> expression)
        {
            var find = await collection.FindAsync(expression);
            return find.ToEnumerable();
        }

        public void Insert(TransferTransaction document)
        {
            collection.InsertOne(document);
        }

        public async Task InsertAsync(TransferTransaction document)
        {
            await collection.InsertOneAsync(document);
        }

        public void Insert(IEnumerable<TransferTransaction> documents)
        {
            collection.InsertMany(documents);
        }

        public async Task InsertAsync(IEnumerable<TransferTransaction> documents)
        {
            await collection.InsertManyAsync(documents);
        }

        public void Delete(Guid id)
        {
            collection.DeleteOne(x => x.id == id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await collection.DeleteOneAsync(x => x.id == id);
        }

        public void Delete(Expression<Func<TransferTransaction, bool>> condition)
        {
            collection.DeleteMany(condition);
        }

        public async Task DeleteAsync(Expression<Func<TransferTransaction, bool>> condition)
        {
            await collection.DeleteManyAsync(condition);
        }

        private UpdateDefinition<TransferTransaction> GetUpdateDefinition(TransferTransaction document)
        {
            List<UpdateDefinition<TransferTransaction>> updates = new List<UpdateDefinition<TransferTransaction>>();
            updates.Add(Builders<TransferTransaction>.Update.Set(x => x.startTime, document.startTime));
            updates.Add(Builders<TransferTransaction>.Update.Set(x => x.endTime, document.endTime));
            updates.Add(Builders<TransferTransaction>.Update.Set(x => x.DestinationAccountNumber, document.DestinationAccountNumber));
            updates.Add(Builders<TransferTransaction>.Update.Set(x => x.SourceAccountNumber, document.SourceAccountNumber));
            updates.Add(Builders<TransferTransaction>.Update.Set(x => x.Status, document.Status));
            updates.Add(Builders<TransferTransaction>.Update.Set(x => x.errorMessage, document.errorMessage));
            updates.Add(Builders<TransferTransaction>.Update.Set(x => x.Amount, document.Amount));

            return Builders<TransferTransaction>.Update.Combine(updates);
        }

        public void Update(TransferTransaction document)
        {
            collection.UpdateOne(x => x.id == document.id, GetUpdateDefinition(document));
        }

        public async Task UpdateAsync(TransferTransaction document)
        {
            await collection.UpdateOneAsync(x => x.id == document.id, GetUpdateDefinition(document));
        }

        public void Update(Expression<Func<TransferTransaction, bool>> expression, TransferTransaction document)
        {
            collection.UpdateMany(expression, GetUpdateDefinition(document));
        }

        public async Task UpdateAsync(Expression<Func<TransferTransaction, bool>> expression, TransferTransaction document)
        {
            await collection.UpdateManyAsync(expression, GetUpdateDefinition(document));
        }

        public void Upsert(TransferTransaction document)
        {
            UpdateOptions updateOptions = new UpdateOptions();
            updateOptions.IsUpsert = true;
            collection.UpdateOne(x => x.id == document.id, GetUpdateDefinition(document), updateOptions);
        }

        public async Task UpsertAsync(TransferTransaction document)
        {
            UpdateOptions updateOptions = new UpdateOptions();
            updateOptions.IsUpsert = true;
            await collection.UpdateOneAsync(x => x.id == document.id, GetUpdateDefinition(document), updateOptions);
        }


    }
}
