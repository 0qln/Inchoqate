using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Inchoqate.GUI.Logging;

/// <summary>
/// Provides a trace writer that writes to a file.
/// </summary>
/// <param name="logger"></param>
public class FileTraceWriter(ILogger logger) : ITraceWriter
{
    /// <inheritdoc />
    public void Trace(TraceLevel level, string message, Exception? ex)
    {
        logger.LogTrace(ex, message);
    }

    /// <inheritdoc />
    public TraceLevel LevelFilter => TraceLevel.Verbose;
}