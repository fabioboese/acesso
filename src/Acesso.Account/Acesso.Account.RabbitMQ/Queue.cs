using Acesso.Account.Domain.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.RabbitMQ
{
    public class Queue<T> : IDisposable, IMessageBrokerQueue<T> where T : class
    {
        private readonly IModel channel;

        internal Queue(string queueName, IConnection connection)
        {
            Name = queueName;
            channel = connection.CreateModel();
            channel.QueueDeclare(queueName, true, false, false, null);
        }

        public string Name { get; private set; }

        public void Dispose()
        {
            channel.Dispose();
            channel.Close();
        }

        public void Publish(T message)
        {
            var json = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(string.Empty, Name, null, bytes);
        }

        public void Subscribe(Action<T> handler)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (chn, e) =>
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());
                T message = JsonConvert.DeserializeObject<T>(json);
                handler(message);
            };
            channel.BasicConsume(Name, true, consumer);
        }
    }
}
