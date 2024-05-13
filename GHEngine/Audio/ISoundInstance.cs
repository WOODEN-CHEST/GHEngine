using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Audio;

namespace GHEngine.Audio;

public interface ISoundInstance
{
    // Fields.
    ISound Sound { get; }
    SoundInstanceState State { get; set; }
    TimeSpan CurrentTime { get; set; }
    bool IsLooped { get; }
    float Volume { get; }
    float Pan { get; }
    double Speed { get; }
    int? LowPassFrequency { get; }
    int? HighPassFrequency { get; }

    public event EventHandler<SoundFinishedArgs>? SoundFinished;
    public event EventHandler<SoundLoopedArgs>? SoundLooped;


    // Methods.
    void GetSamples(float[] buffer, int sampleCount);
}