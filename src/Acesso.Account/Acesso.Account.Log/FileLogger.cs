using Acesso.Account.Domain;
using Acesso.Account.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Acesso.Account.Log
{
    public class FileLogger : ILogger
    {
        private static string filename = null;
        private static Mutex mxFile = new Mutex();

        public FileLogger(IOptions<ApplicationSettings> applicationSettings)
        {
            var path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), applicationSettings.Value.LogLocation);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            Initialize(path);
        }

        private void Initialize(string path)
        {
            filename = System.IO.Path.Combine(path, DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".log");
            System.IO.File.WriteAllText(filename, string.Empty);
        }

        public void Log(string message)
        {
            mxFile.WaitOne();
            System.IO.File.AppendAllText(filename, $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.ffffff} => {message}\r\n");
            mxFile.ReleaseMutex();
        }

        public async Task LogAsync(string message)
        {
            mxFile.WaitOne();
            await System.IO.File.AppendAllTextAsync(filename, $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.ffffff} => {message}\r\n");
            mxFile.ReleaseMutex();
        }

    }
}
