using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GHEngine.IO.DataFile;

public class DataWriteException : Exception
{
    // Constructors.
    public DataWriteException() : base("Failed to write data file due to an unknown reason.") { }

    public DataWriteException(string filePath, string message) 
        : base($"Failed to read file \"{filePath}\". {message}") { }
}