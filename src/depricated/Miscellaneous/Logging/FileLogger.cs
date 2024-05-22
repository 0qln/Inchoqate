using Microsoft.Extensions.Logging;

namespace Miscellaneous.Logging
{
    public class FileLogger(string categoryName, StreamWriter logFileWriter) : ILogger
    {
        private readonly string _categoryName = categoryName;
        private readonly StreamWriter _logFileWriter = logFileWriter;

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return null!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Trace;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);

            _logFileWriter.WriteLine($"[{logLevel}] [{DateTime.Now}] [{_categoryName}] {message}");
            _logFileWriter.Flush();
        }
    }

}
