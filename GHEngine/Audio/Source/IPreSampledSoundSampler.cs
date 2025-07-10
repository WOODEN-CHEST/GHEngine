namespace GHEngine.Audio.Source;

public interface IPreSampledSoundSampler
{
    // Fields.
    float? CustomSampleRate { get; set; }
    double SampleSpeed { get; set; }
    float Volume { get; set; }


    // Methods.
    PreSampledSoundResult Sample(float[] buffer,
        double soundOffset,
        int count, 
        bool isLooped, 
        IPreSampledSound sound,
        WaveFormat targetFormat);
}