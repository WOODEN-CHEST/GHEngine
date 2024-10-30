using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Logging;

public class LoggerLogEventArgs : EventArgs
{
    // Fields.
    public LogLevel Level { get; set; }
    public string Message
    {
        get => _message;
        set => _message = value ?? throw new ArgumentNullException(nameof(value));
    }
    public DateTime TimeStamp { get; set; }


    // Private fields.
    private string _message;


    // Constructors.
    public LoggerLogEventArgs(LogLevel level, string message, DateTime timeStamp)
    {
        Level = level;
        Message = message;
        TimeStamp = timeStamp;
    }
}