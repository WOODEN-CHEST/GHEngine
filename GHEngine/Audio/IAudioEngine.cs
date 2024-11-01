using GHEngine.Audio.Source;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio;

public interface IAudioEngine : IDisposable, ISampleProvider
{
    // Fields.
    float Volume { get; set; }
    int MaxSounds { get; set; }
    int SoundCount { get; }
    int AudioLatency { get; }
    TimeSpan ExecutionTime { get; }
    ISoundInstance[] Sounds { get; }


    // Methods.
    void Start();

    void Stop();

    void AddSoundInstance(ISoundInstance sound);

    void RemoveSoundInstance(ISoundInstance sound);

    void ClearSounds();
}