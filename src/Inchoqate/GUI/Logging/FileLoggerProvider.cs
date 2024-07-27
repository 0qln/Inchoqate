using Microsoft.Extensions.Logging;
using System.IO;

namespace Inchoqate.Logging
{
    public class FileLoggerProvider(StreamWriter logFileWriter) : ILoggerProvider
    {
        private readonly StreamWriter _logFileWriter =
            logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _logFileWriter);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _logFileWriter.Dispose();
        }
    }
}
