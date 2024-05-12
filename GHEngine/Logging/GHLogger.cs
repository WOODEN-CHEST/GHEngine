using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text;

namespace GHEngine.Logging;


public class GHLogger : ILogger
{
    // Internal static fields.
    public string LogPath { get; private set; }


    // Private fields.
    private StreamWriter s_fileWriter;


    // Constructors.
    public GHLogger(string filePath)
    {
        LogPath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        if (!Path.IsPathFullyQualified(LogPath))
        {
            throw new ArgumentException("Path not fully qualified.", nameof(filePath));
        }
        File.Delete(LogPath);

        Directory.CreateDirectory(Path.GetDirectoryName(LogPath) ?? LogPath);

        DateTime Time = DateTime.Now;
        s_fileWriter = new StreamWriter(File.Open(LogPath, FileMode.OpenOrCreate));
        s_fileWriter.Write($"Program instance started on {GetFormattedDate(Time)} at {GetFormattedTime(Time)}. " +
            $"Log generated in \"{LogPath}\"");
    }


    // Private methods.
    private string GetFormattedTime(DateTime time)
    {
        return $"{TwoDigitNumberToString(time.Hour)};{TwoDigitNumberToString(time.Minute)};{TwoDigitNumberToString(time.Second)}";
    }
    private string GetFormattedDate(DateTime date)
    {
        return $"{date.Year}y{TwoDigitNumberToString(date.Month)}m{TwoDigitNumberToString(date.Day)}d";
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
        if (message == null)
        {
            throw new ArgumentNullException(message);
        }

        string FullMessage = $"\n[{GetFormattedTime(DateTime.Now)}{(level == LogLevel.Info ? ' ' : $"[{level}]")} {message}]";
        s_fileWriter.Write(FullMessage);
    }

    public void Dispose()
    {
        s_fileWriter?.Dispose();
    }
}