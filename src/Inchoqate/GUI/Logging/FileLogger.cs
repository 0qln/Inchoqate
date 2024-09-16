using System.IO;
using Microsoft.Extensions.Logging;

namespace Inchoqate.Logging;

public class FileLogger(string categoryName, StreamWriter logFileWriter) : ILogger
{
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
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);

        logFileWriter.WriteLine($"[{logLevel}] [{DateTime.Now}] [{categoryName}] {message}");

        if (exception is not null)
        {
            logFileWriter.WriteLine("=== EXCEPTION ===");
            logFileWriter.WriteLine(exception.ToString());
            logFileWriter.WriteLine("=== END EXCEPTION ===");
        }

        logFileWriter.Flush();
    }
}