using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Logging;

public sealed class ConsoleLogger : ILogger
{
    // Fields.
    public event EventHandler<LoggerLogEventArgs>? LogMessage;


    // Private fields
    private readonly ILogger _wrappedLogger;
    private readonly bool _isWrappedOverConsole;
    private readonly ConsoleColor _defaultColor = Console.ForegroundColor;


    // Constructors.s
    public ConsoleLogger(ILogger? wrappedLogger)
    {
        if (wrappedLogger != null)
        {
            _wrappedLogger = wrappedLogger;
            _isWrappedOverConsole = false;
        }
        else
        {
            _wrappedLogger = new GHLogger(Console.OpenStandardOutput());
            _isWrappedOverConsole = true;
        }
        
    }


    // Methods.
    public void Start()
    {
        _wrappedLogger.LogMessage += OnWrappedLoggerLogEvent;
    }


    // Private methods.
    private void OnWrappedLoggerLogEvent(object? sender, LoggerLogEventArgs args)
    {
        LogMessage?.Invoke(sender, args);
        SetConsoleColor(args.Level);

        if (!_isWrappedOverConsole)
        {
            WriteToConsole(args.Level, args.TimeStamp, args.Message);
        }
    }

    private void SetConsoleColor(LogLevel level)
    {
        Console.ForegroundColor = level switch
        {
            LogLevel.Info => ConsoleColor.Gray,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.CRITICAL => ConsoleColor.DarkRed,
            _ => _defaultColor
        };
    }

    private void WriteToConsole(LogLevel level, DateTime timestamp, string message)
    {
        Console.Write(_wrappedLogger.ConvertToLoggedMessage(level, timestamp, message));
    }


    // Inherited methods.
    public string ConvertToLoggedMessage(LogLevel level, DateTime timeStamp, string message)
    {
        return _wrappedLogger?.ConvertToLoggedMessage(level, timeStamp, message) ?? message;
    }

    public void Critical(string message)
    {
        _wrappedLogger.Critical(message);
    }

    public void Error(string message)
    {
        _wrappedLogger.Error(message);
    }
    public void Warning(string message)
    {
        _wrappedLogger.Warning(message);
    }

    public void Info(string message)
    {
        _wrappedLogger.Info(message);
    }

    public void Log(LogLevel level, string message)
    {
        _wrappedLogger.Log(level, message);
    }

    public void Dispose()
    {
        _wrappedLogger.Dispose();
        Console.ForegroundColor = _defaultColor;
    }
}