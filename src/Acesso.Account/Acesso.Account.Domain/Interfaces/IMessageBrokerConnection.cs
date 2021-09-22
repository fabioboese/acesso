namespace Acesso.Account.Domain.Interfaces
{
    public interface IMessageBrokerConnection
    {
        IMessageBrokerQueue<T> GetQueue<T>(string queueName) where T : class;
    }
}