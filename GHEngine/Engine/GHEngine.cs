using GHEngine.Assets;
using GHEngine.Audio;
using GHEngine.Frame;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using Microsoft.Xna.Framework;


namespace GHEngine.Engine;


public partial class GHEngine : Game
{
    // Fields.
    public string Name { get; private set; }
    public string InternalName { get; private set; }
    public string DataRootDirectory { get; private set; }
    public IDisplay Display { get; private set; }
    public IAssetProvider AssetProvider { get; private set; }
    public ILogger Logger { get; private set; }
    public IAudioEngine AudioEngine { get; private set; }


    // Private fields.
    private GHEngineStartupSettings _startupSettings;
    private GraphicsDeviceManager _graphicsManager;
    private GenericProgramTime _time = new();
    private IGameFrame _activeFrame;


    // Constructors.
    public GHEngine(GHEngineStartupSettings startupSettings)
    {
        _startupSettings  = startupSettings ?? throw new ArgumentNullException(nameof(startupSettings));
    }


    // Private methods.
    private void InitializeLogger()
    {
        string LogPath = Path.IsPathFullyQualified(_startupSettings.LogPath) ? _startupSettings.LogPath :
            Path.Combine(DataRootDirectory, _startupSettings.LogPath);
        string ArchivePath = Path.IsPathFullyQualified(_startupSettings.LogArchiveDirectory) ? _startupSettings.LogArchiveDirectory :
            Path.Combine(DataRootDirectory, _startupSettings.LogArchiveDirectory);
        ILogArchiver Archiver = _startupSettings.LogArchiver ?? new GHLogArchiver();
        Archiver.Archive(ArchivePath, LogPath);
        Logger = _startupSettings.Logger ?? new GHLogger(LogPath);
    }

    private void InitialieAssetProvider()
    {
        GHAssetDefinitionCollection DefinitionCollection = new();
        IAssetDefinitionReader DefinitionReader = _startupSettings.AssetDefinitionReader;


        AssetProvider = _startupSettings.AssetProvider ?? new GHAssetProvider();
    }


    // Inherited methods.
    protected override void Initialize()
    {
        base.Initialize();

        _graphicsManager = new(this);

        DataRootDirectory = _startupSettings.GameDataRootDirectory;
        Display = _startupSettings.Display ?? new GHDisplay(_graphicsManager, Window);
        AudioEngine = _startupSettings.AudioEngine ?? new GHAudioEngine(20);
        InitializeLogger();
        InitialieAssetProvider();
    }

    protected override void Update(GameTime gameTime)
    {
        _time.TotalTimeSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time.PassedTimeSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        UserInput.ListenForInput(IsActive);
        GameFrameManager.UpdateFrames(_time);
        DisplayOld.Update(_time);
    }

    protected override void Draw(GameTime gameTime)
    {
        GameFrameManager.DrawFrames(_time);
    }
}