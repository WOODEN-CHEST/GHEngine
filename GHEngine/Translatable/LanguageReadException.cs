using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Translatable;

public class LanguageReadException : IOException
{
    public LanguageReadException(string message) : base(message) { }
}