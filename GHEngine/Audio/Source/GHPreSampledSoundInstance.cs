using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHEngine.Audio.Modifier;
using NAudio.Wave;
using Tests.Audio;

namespace GHEngine.Audio.Source;

public class GHPreSampledSoundInstance : IPreSampledSoundInstance
{
    // Inherited methods.
    public IPreSampledSoundSampler Sampler
    {
        get => _sampler;
        set => _sampler = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool IsLooped { get; set; }

    public SoundInstanceState State { get; set; } = SoundInstanceState.Playing;

    public ISoundModifier[] Modifiers => _modifiers.ToArray();

    public int ModifierCount => _modifiers.Count;

    public IPreSampledSound Sound { get; private init; }

    public TimeSpan Position
    {
        get => TimeSpan.FromSeconds(_sampleIndex / Sound.Format.SampleRate / Sound.Format.Channels);
        set => _sampleIndex = value.TotalSeconds * Sound.Format.SampleRate;
    }

    public event EventHandler<SoundLoopedArgs>? SoundLooped;
    public event EventHandler<SoundFinishedArgs>? SoundFinished;


    // Private fields.
    private readonly List<ISoundModifier> _modifiers = new();

    private double _sampleIndex = 0d;
    private IPreSampledSoundSampler _sampler = new GHPreSampledSoundSampler();


    // Constructors.
    public GHPreSampledSoundInstance(IPreSampledSound sound)
    {
        Sound = sound ?? throw new ArgumentNullException(nameof(sound));
    }



    // Private methods.
    private bool ApplyModifiers(float[] buffer, int sampleCount, WaveFormat format)
    {
        bool AreMoreSamplesAvailable = false;
        foreach (ISoundModifier Modifier in _modifiers)
        {
            AreMoreSamplesAvailable |= Modifier.Modify(buffer, sampleCount, format);
        }
        return AreMoreSamplesAvailable;
    }

    private void SampleSound(float[] buffer, int sampleCount, WaveFormat format)
    {
        PreSampledSoundResult Result = _sampler.Sample(buffer, _sampleIndex, sampleCount, IsLooped, Sound, format);
        _sampleIndex = Result.NewIndex;

        bool AreMoreSamplesAvailable = ApplyModifiers(buffer, sampleCount, format);

        if (Result.FinishType == PreSampleSoundFinishType.Looped)
        {
            SoundLooped?.Invoke(this, new(this));
        }
        else if (!AreMoreSamplesAvailable && (Result.FinishType == PreSampleSoundFinishType.Finished))
        {
            SoundFinished?.Invoke(this, new(this));
            State = SoundInstanceState.Finished;
        }
    }


    // Inherited methods.
    public void AddModifier(ISoundModifier modifier)
    {
        _modifiers.Add(modifier ?? throw new ArgumentNullException(nameof(modifier)));
    }

    public void ClearModifiers()
    {
        _modifiers.Clear();
    }

    public void InsertModifier(ISoundModifier modifier, int index)
    {
        _modifiers.Insert(index, modifier ?? throw new ArgumentNullException(nameof(modifier)));
    }

    public void RemoveModifier(ISoundModifier modifier)
    {
        _modifiers.Remove(modifier ?? throw new ArgumentNullException(nameof(modifier)));
    }

    public void GetSamples(float[] buffer, int sampleCount, WaveFormat format)
    {
        if (State == SoundInstanceState.Playing)
        {
            SampleSound(buffer, sampleCount, format);
        } 
        else
        {
            Array.Fill(buffer, 0f, 0, sampleCount);
        }
    }
}