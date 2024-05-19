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
            AudioEngine.AddSoundInstance(SoundInstance);

            IUserInput Input = new FakeInput();


            while (true)
            {
                Input.RefreshInput();

                if (Input.WereKeysJustPressed(Keys.Up))
                {
                    SoundInstance.Speed += 0.05f;
                }
                else if (Input.WereKeysJustPressed(Keys.Down))
                {
                    SoundInstance.Speed -= 0.05f;
                }

                Thread.Sleep(1);
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