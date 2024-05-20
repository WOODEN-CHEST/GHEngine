using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.DataFile;

public class DataFileException : Exception
{
    public DataFileException(string message) : base(message) { }
}
