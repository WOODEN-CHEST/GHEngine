using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

internal interface IContinuousSoundSampler
{
    // Fields.
    float? CustomSampleRate { get; set; }
    float Volume { get; set; }


    // Methods.
    void Sample(float[] buffer, int count, IContinuousSound sound);
}