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
    bool IsLooped { get; set; }
    float Volume { get; set; }
    float Pan { get; set; }
    double Speed { get; set; }
    int? LowPassFrequency { get; set; }
    int? HighPassFrequency { get; set; }

    public event EventHandler<SoundFinishedArgs>? SoundFinished;
    public event EventHandler<SoundLoopedArgs>? SoundLooped;


    // Methods.
    void GetSamples(float[] buffer, int sampleCount);
}