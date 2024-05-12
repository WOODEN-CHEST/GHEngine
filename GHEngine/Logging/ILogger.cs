using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Logging;

public interface ILogger : IDisposable
{
    void Info(string message);

    void Warning(string message);
    void Error(string message);

    void Critical(string message);

    void Log(LogLevel level, string message);
}