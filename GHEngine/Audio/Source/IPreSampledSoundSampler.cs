using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public interface IPreSampledSoundSampler
{
    // Fields.
    float? CustomSampleRate { get; set; }
    double SampleSpeed { get; set; }
    float Volume { get; set; }

    public event EventHandler<SoundLoopedArgs>? SoundLooped;
    public event EventHandler<SoundFinishedArgs>? SoundFinished;


    // Methods.
    double Sample(float[] buffer,
        double soundOffset,
        int count, 
        bool isLooped, 
        IPreSampledSound sound,
        WaveFormat targetFormat);
}