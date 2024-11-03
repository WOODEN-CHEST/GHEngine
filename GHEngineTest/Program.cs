using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.Audio.Modifier;
using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using SixLabors.Fonts;
using SixLabors.Fonts.Unicode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace GHEngineTest;

internal class Program
{
    static void Main(string[] args)
    {
        using Game TargetGame = new TestGame();
        TargetGame.Run();
    }
}