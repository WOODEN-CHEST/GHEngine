using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.GameFont;
using GHEngine.IO;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngineTest;

public class TestGame : Game
{
    // Fields.
    private readonly GraphicsDeviceManager _graphicsManager;
    private IUserInput _userInput;
    private IDisplay _display;
    private IFrameRenderer _renderer;
    private IGameFrame _mainFrame;
    private readonly IModifiableProgramTime _time = new GenericProgramTime();
    private readonly HashSet<ITimeUpdatable> _updatables = new();


    // Constructors.
    public TestGame()
    {
        _graphicsManager = new(this);
    }


    // Inherited methods.
    protected override void Initialize()
    {
        base.Initialize();

        
        _userInput = new GHUserInput(Window, this);

        _display = new GHDisplay(_graphicsManager, Window);
        _display.Initialize();
        _renderer = GHRenderer.Create(_graphicsManager.GraphicsDevice, _display);
        _mainFrame = new GHGameFrame();
        _mainFrame.AddLayer(new GHLayer("0"));

        IAudioEngine AudioEngine = new GHAudioEngine(15);
        AudioEngine.Start();

        IAssetStreamOpener StreamOpener = new GHAssetStreamOpener(@"/home/wooden_chest/Desktop/test");
        IAssetDefinitionCollection AssetDefinitions = new GHAssetDefinitionCollection()
        {
            new GHAnimationDefinition("image", new AssetPath[] { new("a", AssetPathType.FileSystem) },  0d, 0, null, false, false),
            new GHFontDefinition("font1", AssetPath.File("font1")),
            new GHFontDefinition("font2", AssetPath.File("font2")),
            new GHSoundDefinition("btfd", AssetPath.File("btfd"))
        };

        GHGenericAssetLoader GenericLoader = new();
        GenericLoader.SetTypeLoader(AssetType.Animation, new AnimationLoader(StreamOpener, GraphicsDevice));
        GenericLoader.SetTypeLoader(AssetType.Font, new FontLoader(StreamOpener, GraphicsDevice));
        GenericLoader.SetTypeLoader(AssetType.Sound, new SoundLoader(StreamOpener, AudioEngine.WaveFormat));
        IAssetProvider Provider = new GHAssetProvider(GenericLoader, AssetDefinitions, null);

        _display.IsUserResizingAllowed = true;
        _userInput.IsMouseVisible = true;

        GHFontFamily FontFamily1 = Provider.GetAsset<GHFontFamily>(_mainFrame, AssetType.Font, "font1")!;
        //GHFontFamily FontFamily2 = Provider.GetAsset<GHFontFamily>(_mainFrame, AssetType.Font, "font2")!;



        WritableTextBox Text = new(_userInput)
        {
            new TextComponent(FontFamily1, "Hello World 1")
            {
                FontSize = 0.25f,
                Mask = Color.Red,
            },
        };
        Text.IsFocused = true;
        Text.Origin = new(0.0f);
        Text.Position = new(0.5f);
        Text.Rotation = 0f;
        Text.Alignment = TextAlignOption.Left;
        Text.IsTypingEnabled = true;
        Text.Rotation = 0f;
        Text.Origin = new(0.5f);

        _mainFrame.Layers[0].AddItem(Text);
        _updatables.Add(Text);
        //_mainFrame.Layers[0].AddItem(new TestBox() { Family = FontFamily1 });
    }

    protected override void BeginRun()
    {
        base.BeginRun();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        _time.PassedTime = gameTime.ElapsedGameTime;
        _time.TotalTime += gameTime.ElapsedGameTime;
        _userInput.RefreshInput();

        foreach (ITimeUpdatable Updatable in _updatables)
        {
            Updatable.Update(_time);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        _renderer.RenderFrame(_mainFrame, _time);
        //((TextBox)_mainFrame.TopLayer!.Items[0]).Rotation = (float)gameTime.TotalGameTime.TotalSeconds;
        base.Draw(gameTime);
    }

    private class TestBox : IRenderableItem
    {
        public required GHFontFamily Family { get; init; }
        public bool IsVisible { get; set; } = true;
        float Rotation;
        public void Render(IRenderer renderer, IProgramTime time)
        {
            float SinValue = MathF.Sin((float)time.TotalTime.TotalSeconds);
            Rotation += (float)time.PassedTime.TotalSeconds;

            renderer.DrawString(new(Family, false, false, 0f, 0f), "Hello World!", new Vector2(0.5f), null,
                Color.Wheat, Rotation, new Vector2(0.5f), new(0.25f), SpriteEffects.None, null, null, null);
        }
    }
}