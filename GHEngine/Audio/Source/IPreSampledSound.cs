namespace GHEngine.Audio.Source;

public interface IPreSampledSound : ISound, IDisposable
{
    // Fields.
    TimeSpan Duration { get; }
    float[] Samples { get; }
    int SampleCount { get; }
    int ChannelSampleCount { get; }
}