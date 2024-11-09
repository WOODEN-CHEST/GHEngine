using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Screen;

public class GHDisplay : IDisplay
{
    // Private static fields.
    private static IntVector MIN_SIZE => new(50, 50);


    // Fields.
    public IntVector WindowedSize
    {
        get => _windowedSize;
        set
        {
            _windowedSize = ClampSize(value);
            if (IsFullScreen || (WindowedSize == CurrentWindowSize))
            {
                return;
            }
            UpdateDisplay(WindowedSize, IsFullScreen);
        }
    }

    public IntVector FullScreenSize
    {
        get => _fullScreenSize;
        set
        {
            _fullScreenSize = ClampSize(value);
            if (!IsFullScreen || (FullScreenSize == CurrentWindowSize))
            {
                return;
            }
            UpdateDisplay(FullScreenSize, IsFullScreen);
        }
    }

    public bool IsFullScreen
    {
        get => _graphics.IsFullScreen;
        set
        {
            if (_graphics.IsFullScreen == value)
            {
                return;
            }
            UpdateDisplay(value ? FullScreenSize : WindowedSize, value);
        }
    }

    public bool IsUserResizingAllowed
    {
        get => _window.AllowUserResizing;
        set => _window.AllowUserResizing = value;
    }

    public IntVector CurrentWindowSize
    {
        get => new IntVector(_window.ClientBounds.Width, _window.ClientBounds.Height);
        set
        {
            IntVector NewSize = ClampSize(value);
            if (NewSize != CurrentWindowSize)
            {
                UpdateDisplay(NewSize, IsFullScreen);
            }
        }
    }

    public IntVector ScreenSize
    {
        get
        {
            DisplayMode Mode = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode;
            return new IntVector(Mode.Width, Mode.Height);
        }
    }

    public event EventHandler<ScreenSizeChangeEventArgs>? ScreenSizeChange;


    // Private fields.
    private IntVector _windowedSize;
    private IntVector _fullScreenSize;
    private readonly GraphicsDeviceManager _graphics;
    private GameWindow _window;




    // Constructors.
    public GHDisplay(GraphicsDeviceManager manager, GameWindow window)
    {
        _graphics = manager ?? throw new ArgumentNullException(nameof(manager));
        _window = window ?? throw new ArgumentNullException(nameof(window));

        WindowedSize = new IntVector(ScreenSize.X / 2, ScreenSize.Y / 2);
        IsFullScreen = false;
    }


    // Private methods.
    private void OnWindowSizeChangeEvent(object? sender, EventArgs args)
    {
        if (!IsFullScreen)
        {
            WindowedSize = CurrentWindowSize;
        }
        else
        {
            FullScreenSize = CurrentWindowSize;
        }

        ScreenSizeChange?.Invoke(this, new ScreenSizeChangeEventArgs(CurrentWindowSize));
    }

    private void UpdateDisplay(IntVector size, bool isFullScreen)
    {
        _graphics.IsFullScreen = isFullScreen;
        _graphics.PreferredBackBufferWidth = size.X;
        _graphics.PreferredBackBufferHeight = size.Y;
        _graphics.ApplyChanges();
    }

    private IntVector ClampSize(IntVector size)
    {
        return new IntVector(Math.Max(size.X, MIN_SIZE.X), Math.Max(size.Y, MIN_SIZE.Y));
    }

    public void Initialize()
    {
        _window.ClientSizeChanged += OnWindowSizeChangeEvent;
    }

    public void Dispose()
    {
        _window.ClientSizeChanged -= OnWindowSizeChangeEvent;
    }
}