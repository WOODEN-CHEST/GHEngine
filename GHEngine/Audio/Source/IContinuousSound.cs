using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public interface IContinuousSound : ISound
{
    // Methods.
    void FeedSamples(float[] samples);
}