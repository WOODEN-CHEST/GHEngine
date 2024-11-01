using GHEngine.Audio.Modifier;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Audio;

namespace GHEngine.Audio.Source;

public interface ISoundInstance
{
    // Fields.
    SoundInstanceState State { get; set; }
    ISoundModifier[] Modifiers { get; }
    int ModifierCount { get; }

    public event EventHandler<SoundFinishedArgs>? SoundFinished;


    // Methods.
    void GetSamples(float[] buffer, int sampleCount, WaveFormat targetFormat);

    void AddModifier(ISoundModifier modifier);
    void InsertModifier(ISoundModifier modifier, int index);
    void RemoveModifier(ISoundModifier modifier);
    void ClearModifiers();
}