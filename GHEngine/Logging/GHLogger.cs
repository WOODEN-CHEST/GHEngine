using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection.Emit;
using System.Text;

namespace GHEngine.Logging;


public class GHLogger : ILogger
{
    // Internal static fields.
    public event EventHandler<LoggerLogEventArgs>? LogMessage;


    // Private fields.
    private readonly StreamWriter _fileWriter;
    private readonly bool _openedStream;
    private bool _isDisposed;


    // Constructors.
    public GHLogger(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));
        if (!Path.IsPathFullyQualified(filePath))
        {
            throw new ArgumentException("Path not fully qualified.", nameof(filePath));
        }
        File.Delete(filePath);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? filePath);
        _fileWriter = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate));
        _openedStream = true;

    }

    public GHLogger(Stream fileStream)
    {
        _fileWriter = new StreamWriter(fileStream ?? throw new ArgumentNullException(nameof(fileStream)));
        _openedStream = false;
    }


    // Private methods.
    private string GetFormattedTime(DateTime time)
    {
        return $"{TwoDigitNumberToString(time.Hour)};{TwoDigitNumberToString(time.Minute)};{TwoDigitNumberToString(time.Second)}";
    }

    private string TwoDigitNumberToString(int number)
    {
        return number < 10 ? $"0{number}" : number.ToString();
    }


    // Inherited methods.
    public void Info(string message) => Log(LogLevel.Info, message);

    public void Warning(string message) => Log(LogLevel.Warning, message);

    public void Error(string message) => Log(LogLevel.Error, message);

    public void Critical(string message) => Log(LogLevel.CRITICAL, message);

    public void Log(LogLevel level, string message)
    {
        lock (this)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(message, nameof(message));
            LoggerLogEventArgs Args = new(level, message, DateTime.Now);
            LogMessage?.Invoke(this, Args);

            _fileWriter.Write(ConvertToLoggedMessage(Args.Level, Args.TimeStamp, Args.Message));
            _fileWriter.Flush();
        }
    }

    public string ConvertToLoggedMessage(LogLevel level, DateTime timeStamp, string message)
    {
        return $"\n[{GetFormattedTime(timeStamp)}{(level == LogLevel.Info ? null : $"[{level}]")} {message}]";
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        if (_openedStream)
        {
            _fileWriter?.Dispose();
        }
        _isDisposed = true;
    }
}