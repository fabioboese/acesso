using Acesso.Account.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acesso.Account.Log
{
    public static class Setup
    {
        public static void AddLogger(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, FileLogger>();
        }
    }
}
