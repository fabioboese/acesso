using Acesso.Account.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.RabbitMQ
{
    public static class Setup
    {
        public static void AddRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBrokerConnection, Connection>();
        }

    }
}
