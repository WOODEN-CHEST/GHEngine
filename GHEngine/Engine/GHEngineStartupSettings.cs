using GHEngine.Assets;
using GHEngine.Audio;
using GHEngine.Frame;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;


namespace GHEngine.Engine;


public record class GHEngineStartupSettings
{
    // Fields.
    public string InternalName { get; init; } = "Unnamed_Game";
    public string GameName { get; init; } = "Game";

    public string GameDataRootDirectory
    {
        get => _gameDataRootDirectory
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), InternalName);
        set => _gameDataRootDirectory = value;
    }

    public IAssetProvider? AssetProvider { get; init; } = null;
    public IAssetLoader? AssetLoader { get; init; } = null;
    public IAssetDefinitionReader? AssetDefinitionReader { get; init; } = null;
    public string AssetDefinitionDirectory { get; init; }

    public float AspectRatio { get; init; } = IDisplay.ASPECT_RATIO_STANDART;
    public IDisplay? Display { get; init; } = null;
    public bool AllowUserResizing { get; set; } = true;
    public bool IsFullScreen { get; set; } = true;

    public Func<GHEngineOld, IGameFrame> StartupFrameProvider { get; set; }

    public IUserInput? UserInput { get; init; } = null;
    public bool IsMouseVisible { get; set; } = true;
    public bool AllowAltF4 { get; set; } = true;

    public ILogger? Logger { get; init; } = null;
    public ILogArchiver? LogArchiver { get; init; } = null;
    public string LogPath { get; init; } = "logs/latest.log";
    public string LogArchiveDirectory { get; init; } = "logs/old";

    public IAudioEngine? AudioEngine { get; init; } = null;
    


    // Private fields.
    private string? _gameDataRootDirectory;


    // Constructors.
    public GHEngineStartupSettings(Func<GHEngineOld, IGameFrame> startingFrameProvider)
    {
        StartupFrameProvider = startingFrameProvider ?? throw new ArgumentNullException(nameof(startingFrameProvider));
        AssetDefinitionDirectory = Path.Combine(Environment.CurrentDirectory, "assets");
    }
}