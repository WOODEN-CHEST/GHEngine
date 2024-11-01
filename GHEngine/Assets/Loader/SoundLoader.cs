using GHEngine.Assets.Def;
using GHEngine.Audio;
using GHEngine.Audio.Source;
using NAudio.Wave;

namespace GHEngine.Assets.Loader;

public class SoundLoader : GHStreamAssetLoader
{
    // Constructors,
    public SoundLoader(IAssetStreamOpener streamOpener) : base(streamOpener) { }


    // Private methods.
    private GHPreSampledSound LoadAudio(GHSoundDefinition definition)
    {
        Mp3FileReader Reader = null!;

        ISampleProvider Sampler;
        try
        {
            AssetPath FullPath = new(Path.Combine(definition.Type.RootPathName, definition.TargetPath.Path),
                definition.TargetPath.Type);
            Reader = new(StreamOpener.GetStream(FullPath));
            {
                Sampler = Reader.ToSampleProvider();
            }

            float[] Samples = new float[(int)Math.Ceiling(Reader.TotalTime.TotalSeconds
                * Sampler.WaveFormat.Channels * Sampler.WaveFormat.SampleRate)];
            Sampler.Read(Samples, 0, Samples.Length);
            Reader.Dispose();

            return new GHPreSampledSound(Sampler.WaveFormat, Samples);
        }
        catch (IOException e)
        {
            Reader?.Dispose();
            throw new AssetLoadException(definition, $"Couldn't read sound file. {e}");
        }
    }


    // Inherited methods.
    public override object Load(AssetDefinition definition)
    {
        if (definition is not GHSoundDefinition SoundDefinition)
        {
            throw new AssetLoadException("Asset definition is not a sound definition.");
        }

        return LoadAudio(SoundDefinition);
    }
}