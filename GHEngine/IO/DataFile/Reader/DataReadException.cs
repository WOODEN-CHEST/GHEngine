using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GHEngine.IO.DataFile;

public class DataReadException : Exception
{
    // Constructors.
    public DataReadException() : base("Failed to read file data due to an unknown reason.") { }

    public DataReadException(string message) : base(message) { }

    public DataReadException(string filePath, string message) 
        : base($"Failed to read the file \"{filePath}\". {message}") { }
}