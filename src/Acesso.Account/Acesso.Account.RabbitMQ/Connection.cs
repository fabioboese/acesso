using Acesso.Account.Domain;
using Acesso.Account.Domain.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;


namespace Acesso.Account.RabbitMQ
{
    public class Connection : IDisposable, IMessageBrokerConnection
    {
        private readonly IConnection connection;
        private ConnectionFactory factory = new ConnectionFactory();
        private Dictionary<string, object> queues = new Dictionary<string, object>();

        public Connection(IOptions<ApplicationSettings> applicationSettings)
        {
            // connection string format: "amqp://user:pass@hostName:port/vhost"
            factory.Uri = new Uri(applicationSettings.Value.MessageBrokerConnectionString);
            connection = factory.CreateConnection();
        }

        public IMessageBrokerQueue<T> GetQueue<T>(string queueName) where T : class
        {
            if (!queues.ContainsKey(queueName))
            {
                var queue = new Queue<T>(queueName, connection);
                queues.Add(queueName, queue);
            }
            
            return (IMessageBrokerQueue<T>)queues[queueName];
        }

        public void Dispose()
        {
            connection.Dispose();
            connection.Close();
        }
    }
}
