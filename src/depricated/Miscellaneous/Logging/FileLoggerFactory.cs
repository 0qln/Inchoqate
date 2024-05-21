using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miscellaneous.Logging
{
    public static class FileLoggerFactory
    {
        private static readonly ILoggerFactory _factory;
        private static readonly StreamWriter _writer;

        static FileLoggerFactory()
        {
            string logFilePath = "log.txt";
            _writer = new(logFilePath, append: true);
            _factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new FileLoggerProvider(_writer));
            });

            _factory
                .CreateLogger("Logging")
                .LogInformation("File logger initiated.");
        }

        public static ILogger<T> CreateLogger<T>()
        {
            return _factory.CreateLogger<T>();
        }
    }
}
