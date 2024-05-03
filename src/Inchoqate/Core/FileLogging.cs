using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core;

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
        _logFileWriter.Dispose();
    }
}

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
