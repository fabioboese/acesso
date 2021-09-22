using System;

namespace Acesso.Account.Domain.Interfaces
{
    public interface IMessageBrokerQueue<T> where T : class
    {
        string Name { get; }

        void Publish(T message);
        void Subscribe(Action<T> handler);
    }
}