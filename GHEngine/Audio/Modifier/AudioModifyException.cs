using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Modifier;

public class AudioModifyException : Exception
{
    public AudioModifyException(string message) : base(message) { }
    public AudioModifyException(string message, Exception innerException) : base(message, innerException) { }
}