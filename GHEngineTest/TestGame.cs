using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Frame;
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

        IAssetStreamOpener StreamOpener = new GHAssetStreamOpener(@"C:\Users\User\Desktop\test");
        IAssetDefinitionCollection AssetDefinitions = new GHAssetDefinitionCollection()
        {
            new GHAnimationDefinition("image", new AssetPath[] { new("a", AssetPathType.FileSystem) },  0d, 0, null, false, false),
            new GHFontDefinition("font", AssetPath.File("font"))
        };

        GHGenericAssetLoader GenericLoader = new();
        GenericLoader.SetTypeLoader(AssetType.Animation, new AnimationLoader(StreamOpener, GraphicsDevice));
        GenericLoader.SetTypeLoader(AssetType.Font, new FontLoader(StreamOpener, GraphicsDevice));
        IAssetProvider Provider = new GHAssetProvider(GenericLoader, AssetDefinitions, null);

        _display.IsUserResizingAllowed = true;
        _userInput.IsMouseVisible = true;

        GHFontFamily FontFamily = Provider.GetAsset<GHFontFamily>(_mainFrame, AssetType.Font, "font")!;

        //TextBox Text = new();
        //Text.Add(new TextComponent(, "Hello World"!));


        //_mainFrame.Layers[0].AddItem(Text);
        _mainFrame.Layers[0].AddItem(new TestBox() { Family = FontFamily });
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

        if (_userInput.WereMouseButtonsJustReleased(MouseButton.Middle))
        {
            Exit();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        _renderer.RenderFrame(_mainFrame, _time);
        base.Draw(gameTime);
    }

    private class TestBox : IRenderableItem
    {
        public required GHFontFamily Family { get; init; }
        public bool IsVisible { get; set; } = true;

        public void Render(IRenderer renderer, IProgramTime time)
        {
            string TextDouble = "Hello World W";
            string Text = "Hello World 1";

            renderer.DrawString(new(Family, false, false, 0f, 0f), TextDouble, new(0.0f, 0.0f), Color.White,
                0f, new(0.5f, 0.5f), new Vector2(0.9f, 0.9f), SpriteEffects.None, null, null, null);
        }
    }
}