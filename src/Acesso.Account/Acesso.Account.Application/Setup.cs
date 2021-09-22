using Acesso.Account.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Application
{
    public static class Setup
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<ITransferTransactionApp, TransferTransaction>();
        }
    }
}
