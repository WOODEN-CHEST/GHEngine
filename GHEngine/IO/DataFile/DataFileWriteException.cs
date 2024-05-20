using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GHEngine.IO.DataFile;

public class DataFileWriteException : DataFileException
{
    // Constructors.
    public DataFileWriteException() : base("Failed to write data file due to an unknown reason.") { }

    public DataFileWriteException(string message) : base($"Failed to write data file. {message}") { }

    public DataFileWriteException(string filePath, string message) 
        : base($"Failed to write data file \"{filePath}\". {message}") { }
}