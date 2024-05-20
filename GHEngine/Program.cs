using GHEngine.Assets;
using GHEngine.Audio;
using GHEngine.IO;
using GHEngine.IO.JSON;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GHEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IAudioEngine AudioEngine = new GHAudioEngine(20);
            string AssetDir = @"C:\Users\User\Desktop\assets";

            IAssetLoader Loader = new GHAssetLoader(AssetDir, new FakeProvider(), AudioEngine.WaveFormat);
            ISound Sound = (ISound)Loader.Load(new GHSongDefinition("test"));
            ISoundInstance SoundInstance = Sound.CreateInstance();
            SoundInstance.IsLooped = true;
            SoundInstance.Speed = 1d;
            SoundInstance.CurrentTime = new TimeSpan(0, 0, 1, 22, 0);
            SoundInstance.LowPassFrequency = null;
            SoundInstance.Volume = 0.5f;
            SoundInstance.Pan = 0f;
            AudioEngine.AddSoundInstance(SoundInstance);


            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        private class FakeProvider : IServiceProvider
        {
            public object? GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }
    }
}