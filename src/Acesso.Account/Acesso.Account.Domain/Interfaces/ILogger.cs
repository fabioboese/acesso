using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Acesso.Account.Domain.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        Task LogAsync(string message);
    }
}
