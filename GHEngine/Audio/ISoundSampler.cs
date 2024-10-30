using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio;

public interface ISoundSampler
{
    // Fields.


    // Methods.
    public void Read(float[] buffer, int count);
}