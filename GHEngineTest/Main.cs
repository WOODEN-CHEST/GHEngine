using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.Audio.Modifier;
using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace GHEngineTest;

internal class Program
{
    static void Main(string[] args)
    {
        using Game TargetGame = new TestGame();
        TargetGame.Run();
    }
}