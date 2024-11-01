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
        get
        {
            lock (this)
            {
                return _sampler;
            }
        }
        set
        {
            lock (this)
            {
                _sampler = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }

    public bool IsLooped
    {
        get
        {
            lock (this)
            {
                return _isLooped;
            }
        }
        set
        {
            lock (this)
            {
                _isLooped = value;
            }
        }
    }

    public SoundInstanceState State
    {
        get
        {
            lock (this)
            {
                return _state;
            }
        }
        set
        {
            lock (this)
            {
                _state = value;
            }
        }
    }
    
    public ISoundModifier[] Modifiers
    {
        get
        {
            lock (this)
            {
                return _modifiers.ToArray();
            }
        }
    }

    public int ModifierCount => _modifiers.Count;

    public IPreSampledSound Sound { get; private init; }
    public TimeSpan Position
    {
        get
        {
            lock (this)
            {
                return TimeSpan.FromSeconds(_sampleIndex / Sound.Format.SampleRate / Sound.Format.Channels);
            }
        }
        set
        {
            double NewIndex = value.TotalSeconds * Sound.Format.SampleRate;
            lock (this)
            {
                _sampleIndex = NewIndex;
            }
        }
    }

    public event EventHandler<SoundLoopedArgs>? SoundLooped;
    public event EventHandler<SoundFinishedArgs>? SoundFinished;


    // Private fields.
    private readonly List<ISoundModifier> _modifiers = new();

    private bool _isLooped = false;
    private double _sampleIndex = 0d;
    private SoundInstanceState _state = SoundInstanceState.Playing;
    private IPreSampledSoundSampler _sampler = new GHPreSampledSoundSampler();


    // Constructors.
    public GHPreSampledSoundInstance(IPreSampledSound sound)
    {
        Sound = sound ?? throw new ArgumentNullException(nameof(sound));
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
        double SampleIndexNow;
        bool IsLoopedNow;
        ISoundModifier[] ActiveModifiers;
        lock (this)
        {
            SampleIndexNow = _sampleIndex;
            IsLoopedNow = _isLooped;
            ActiveModifiers = _modifiers.Count == 0 ? Array.Empty<ISoundModifier>() : _modifiers.ToArray();
        }

        double NewIndex = _sampler.Sample(buffer, SampleIndexNow, sampleCount, _isLooped, Sound, format);

        lock (this)
        {
            _sampleIndex = NewIndex;
        }

        foreach (ISoundModifier Modifier in _modifiers)
        {
            Modifier.Modify(buffer, sampleCount, format);
        }
    }
}