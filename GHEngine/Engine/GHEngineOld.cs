using GHEngine.Assets;
using GHEngine.Audio;
using GHEngine.Frame;
using GHEngine.Logging;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace GHEngine.Engine;


public class GHEngineOld
{
    // Fields.





    // Private fields.
    private GHEngine _game;
    private GHEngineStartupSettings _startupSettings;


    // Constructors.
    public GHEngineOld(GHEngineStartupSettings settings)
    {
        _startupSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        _game = new GHEngineGame();
        _game.


        Display = new GHDisplay()

    }


    // Methods.
    public void Execute(GHEngineStartupSettings settings)
    {
        if (Game != null)
        {
            throw new InvalidOperationException("Engine already initialized!");
        }

        StartupSettings = settings ?? throw new ArgumentNullException(nameof(settings));

        GameName = StartupSettings.GameName;
        InternalName = StartupSettings.InternalName;

        DataRootPath = Path.Combine(StartupSettings.GameDataRootDirectory, InternalName);
        if (!Path.IsPathFullyQualified(DataRootPath))
        {
            throw new ArgumentException("Not a fully qualified data root path.", nameof(settings.GameDataRootDirectory));
        }
        Directory.CreateDirectory(DataRootPath);

        GHLogger.Initialize(Path.Combine(DataRootPath, LOGS_SUBDIRECTORY));

        try
        {
            using (Game = new GHEngineGame())
            {
                Game.Run();
            }
            Stop();
        }
        catch (Exception e)
        {
            OnCrash(e);
        }
    }

    public static void Exit()
    {
        Game.Exit();
    }


    // Private static methods.
    private static void OnCrash(Exception e)
    {
        GHLogger.Critical($"Game has crashed! " +
        $"Main thread ID: {Environment.CurrentManagedThreadId}. Info: {e}");
        Stop();

        if (OperatingSystem.IsWindows())
        {
            Process.Start("notepad", GHLogger.LogPath);
        }
    }

    private static void Stop()
    {
        GHLogger.Stop();
        GHAudioEngine.ActiveEngine.Dispose();
    }
}