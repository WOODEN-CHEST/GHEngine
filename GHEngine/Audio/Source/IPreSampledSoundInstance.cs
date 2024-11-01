using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public interface IPreSampledSoundInstance : ISoundInstance
{
    // Fields.
    IPreSampledSound Sound { get; }
    IPreSampledSoundSampler Sampler { get; set; }
    bool IsLooped { get; set; }
    TimeSpan Position { get; set; }

    event EventHandler<SoundLoopedArgs>? SoundLooped;
}