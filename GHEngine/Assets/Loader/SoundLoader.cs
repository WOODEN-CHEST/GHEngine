using GHEngine.Assets.Def;
using GHEngine.Audio.Source;
using NAudio.Wave;
using NVorbis;


namespace GHEngine.Assets.Loader;


public class SoundLoader : GHStreamAssetLoader
{
    // Static fields.
    public const string FILE_EXTENSION = ".ogg";


    // Private fields.
    private readonly WaveFormat _targetFormat;


    // Constructors.
    public SoundLoader(IAssetStreamOpener streamOpener, WaveFormat targetFormat) : base(streamOpener)
    {
        _targetFormat = targetFormat ?? throw new ArgumentNullException(nameof(targetFormat));
    }


    // Private methods.
    private float[] MergeSamplesDown(float[] samples, int channelCount)
    {
        throw new NotSupportedException("GHEngine currently does not support merging down audio channels.");
    }

    private float[] ExtendSamples(float[] samples, int channelCount)
    {
        int ChannelSampleCount = samples.Length / channelCount;
        float[] NewSamples = new float[ChannelSampleCount * _targetFormat.Channels];

        for (int i = 0; i < ChannelSampleCount; i++)
        {
            int BaseIndexOld = i * channelCount;
            int BaseIndexNew = i * _targetFormat.Channels;

            int Channel;
            for (Channel = 0; Channel < channelCount; Channel++)
            {
                NewSamples[BaseIndexNew + Channel] = samples[BaseIndexOld + Channel];
            }
            for (;  Channel < _targetFormat.Channels; Channel++)
            {
                NewSamples[BaseIndexNew + Channel] = samples[BaseIndexOld + (Channel % channelCount)];
            }
        }

        return NewSamples;
    }

    private float[] SampleAudio(VorbisReader reader)
    {
        float[] Samples = new float[reader.TotalSamples * _targetFormat.Channels];
        reader.ReadSamples(Samples, 0, Samples.Length);

        if (_targetFormat.Channels > reader.Channels)
        {
            return ExtendSamples(Samples, reader.Channels);
        }
        else if (_targetFormat.Channels < reader.Channels)
        {
            return MergeSamplesDown(Samples, reader.Channels);
        }
        return Samples;
    }

    private GHPreSampledSound LoadAudio(GHSoundDefinition definition)
    {
        try
        {
            string ModifiedPath = Path.Combine(definition.Type.RootPathName, definition.TargetPath.Path);
            if (definition.TargetPath.Type == AssetPathType.FileSystem)
            {
                ModifiedPath = Path.ChangeExtension(ModifiedPath, FILE_EXTENSION);
            }
            AssetPath FullPath = new(ModifiedPath, definition.TargetPath.Type);

            using VorbisReader Reader = new(StreamOpener.GetStream(FullPath), false);
            float[] Samples = SampleAudio(Reader);

            return new GHPreSampledSound(WaveFormat.CreateIeeeFloatWaveFormat(Reader.SampleRate, Reader.Channels), Samples);
        }
        catch (IOException e)
        {
            throw new AssetLoadException(definition, $"Couldn't read sound file. {e}");
        }
    }


    // Inherited methods.
    public override IDisposable Load(AssetDefinition definition)
    {
        if (definition is not GHSoundDefinition SoundDefinition)
        {
            throw new AssetLoadException("Asset definition is not a sound definition.");
        }

        return LoadAudio(SoundDefinition);
    }
}