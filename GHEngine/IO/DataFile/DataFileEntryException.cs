using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.DataFile;

public class DataFileEntryException : DataFileException
{
    public DataFileEntryException(string message) : base(message) { }
}