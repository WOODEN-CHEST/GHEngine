﻿using GHEngine.Audio;
using GHEngine.Frame;
using GHEngine.IO;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace GHEngine.Engine;


public partial class GHEngineGame : Game
{
    // Internal fields.
    internal GraphicsDeviceManager GraphicsManager { get; private set; }


    // Private fields.
    private GenericProgramTime _time = new();


    // Constructors.
    public GHEngineGame()
    {
        DisplayOld.GraphicsManager = new(this);
    }


    // Inherited methods.
    protected override void Initialize()
    {
        base.Initialize();

        // Create engine components.
        /* Display. */
        DisplayOld.Initialize(Window);

        /* User input. */
        Window.FileDrop += UserInput.OnFileDropEvent;
        Window.TextInput += UserInput.OnTextInputEvent;

        /*  Audio engine. */
        AudioEngine.ActiveEngine = new();

        /* Asset manager. */
        AssetManager.Initialize(GHEngine.StartupSettings!.AssetBasePath);


        // Set fields.
        IsMouseVisible = GHEngine.StartupSettings.IsMouseVisible;
        Window.AllowAltF4 = GHEngine.StartupSettings.AllowAltF4;
        Window.AllowUserResizing = GHEngine.StartupSettings.AllowUserResizing;

        // Start frames.
        GameFrameManager.Initialize(GHEngine.StartupSettings.StartupFrame);
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