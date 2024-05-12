using GHEngine.Audio;
using GHEngine.Logging;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace GHEngine.Engine;


public class GHEngine
{
    // Static fields.
    public static Game Game { get; private set; }
    public string GameName { get; private set; }
    public string InternalName { get; private set; }
    public string DataRootPath { get; private set; }
    public static GHEngineStartupSettings? StartupSettings { get; private set; }


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
        AudioEngine.ActiveEngine.Dispose();
    }
}