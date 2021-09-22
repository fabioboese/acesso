using Microsoft.Extensions.DependencyInjection;
using System;
using Acesso.Account.RabbitMQ;
using Acesso.Account.MongoDB;
using Acesso.Account.Domain;
using Acesso.Account.Application;
using Microsoft.Extensions.Options;
using Acesso.Account.Log;

namespace Acesso.Account.IoC
{
    public static class Setup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddRabbitMQ();
            services.AddMongoDB();
            services.AddLogger();
            services.AddApplication();
        }
    }
}
