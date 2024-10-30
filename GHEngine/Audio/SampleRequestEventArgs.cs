using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio;

public class SampleRequestEventArgs : EventArgs
{
    // Fields.
    public byte[] Buffer { get; }
    public int Count { get; }


    // Constructors.
    public SampleRequestEventArgs(byte[] buffer, int count)
    {
        Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        Count = count;
    }
}