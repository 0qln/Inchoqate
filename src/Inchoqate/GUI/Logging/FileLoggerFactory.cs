using Microsoft.Extensions.Logging;
using System.IO;
using Inchoqate.GUI.Logging;
using Newtonsoft.Json.Serialization;

namespace Inchoqate.Logging;

/// <summary>
/// File logger factory.
/// </summary>
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

    /// <summary>
    /// Creates a new <see cref="Logger"/> with a <see cref="FileLogger"/> as backing logger.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ILogger<T> CreateLogger<T>()
    {
        return _factory.CreateLogger<T>();
    }

    /// <summary>
    /// Creates a new <see cref="FileTraceWriter"/> instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ITraceWriter CreateTraceWriter<T>()
    {
        return new FileTraceWriter(CreateLogger<T>());
    }
}