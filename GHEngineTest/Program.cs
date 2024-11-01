using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.Audio.Source;

namespace GHEngineTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IAudioEngine AudioEngine = new GHAudioEngine(15);

            IAssetStreamOpener StreamOpener = new GHAssetStreamOpener(@"C:\Users\User\Desktop\test");
            IAssetLoader AudioLoader = new SoundLoader(StreamOpener);

            AssetDefinition Definition = new GHSoundDefinition("back from dead", AssetPath.File("bftd.mp3"));

            IPreSampledSound Music = (IPreSampledSound)AudioLoader.Load(Definition);
            IPreSampledSoundInstance MusicInstance = (IPreSampledSoundInstance)Music.CreateInstance();
            MusicInstance.IsLooped = true;
            MusicInstance.Sampler.SampleSpeed = 1d;
            MusicInstance.Sampler.CustomSampleRate = 8000;
            MusicInstance.Sampler.Volume = 1f;

            AudioEngine.AddSoundInstance(MusicInstance);
            AudioEngine.Start();

            while (true)
            {
                Thread.Sleep(10_000);
            }
        }
    }
}
