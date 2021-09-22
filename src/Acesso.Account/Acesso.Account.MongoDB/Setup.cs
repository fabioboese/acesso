using Acesso.Account.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.MongoDB
{
    public static class Setup
    {
        public static void AddMongoDB(this IServiceCollection services)
        {
            services.AddSingleton<ITransferTransactionRepository, TransferTransactionRepository>();
        }

    }
}
