using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.IO;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
        _renderer = new GHRenderer(_graphicsManager.GraphicsDevice, _display);
        _renderer.Initialize();
        _mainFrame = new GHGameFrame();
        _mainFrame.AddLayer(new GHLayer("0"));

        IAssetStreamOpener StreamOpener = new GHAssetStreamOpener(@"C:\Users\User\Desktop\test");
        IAssetDefinitionCollection AssetDefinitions = new GHAssetDefinitionCollection()
        {
            new GHAnimationDefinition("image", new AssetPath[] { new("a", AssetPathType.FileSystem) },  0d, 0, null, false, false)
        };

        GHGenericAssetLoader GenericLoader = new();
        GenericLoader.SetTypeLoader(AssetType.Animation, new AnimationLoader(StreamOpener, GraphicsDevice));
        IAssetProvider Provider = new GHAssetProvider(GenericLoader, AssetDefinitions, null);

        _display.IsUserResizingAllowed = true;
        _userInput.IsMouseVisible = true;

        int i = 0;

        //SpriteItem Item = new SpriteItem(Provider.GetAsset<ISpriteAnimation>(
        //    _mainFrame, AssetType.Animation, "image")!.CreateInstance());
        //Item.Size = Vector2.One / Item.FrameSize * 10f;
        //_mainFrame.Layers[0].AddItem(Item);
        _mainFrame.Layers[0].AddItem(new TestBox());
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
        public bool IsVisible { get; set; } = true;

        public void Render(IRenderer renderer, IProgramTime time)
        {
            renderer.DrawRectangle(Color.Red, new(0f, 0f), new(1f, 1f), Vector2.Zero, 0f, null);
        }
    }
}