using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
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

        ISpriteAnimation Animation = Provider.GetAsset<ISpriteAnimation>(_mainFrame, AssetType.Animation, "image")!;
        //_mainFrame.Layers[0].AddItem(Text);
        _mainFrame.Layers[0].AddItem(new TestBox() { Family = FontFamily, Anim = Animation.CreateInstance() });
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
        public required IAnimationInstance Anim { get; init; }
        public required GHFontFamily Family { get; init; }
        public bool IsVisible { get; set; } = true;
        float Rotation;

        public void Render(IRenderer renderer, IProgramTime time)
        {
            float SinValue = MathF.Sin((float)time.TotalTime.TotalSeconds);
            Rotation += (float)time.PassedTime.TotalSeconds;

            string TextDouble = "Hello World\nHello World 2";
            //string Text = "Hello World 1";
            //renderer.DrawRectangle(Color.Red, new Vector2(0.5f, 0f), new Vector2(0.5f + 0.5f * SinValue, 1f),
            //    Vector2.Zero, 0f, null, null);

            renderer.DrawString(new(Family, false, false, 0f, 0f), TextDouble, new(0.3f, 0.3f), new RectangleF(0.2f, 0.2f, 0.8f, 0.8f),
                Color.White, Rotation, new(0.5f, 0.5f), new Vector2(0.4f, 0.4f), SpriteEffects.None, null, null, null);

            //renderer.DrawSprite(Anim.CurrentFrame, new Vector2(0.5f, 0.5f), new RectangleF(0.3f, 0.3f, 0.5f, 0.5f),
            //    Color.White, Rotation, new Vector2(1.0f, 1.0f),
            //    new Vector2(0.5f, 0.5f), SpriteEffects.None, null, null);

        }
    }
}