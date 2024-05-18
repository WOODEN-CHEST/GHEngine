using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Screen;

public class GHDisplay : IDisplay
{
    // Private static fields.
    private static WindowSize MIN_SIZE => new(50, 50);


    // Fields.
    public float TargetAspectRatio { get; set; }

    public WindowSize WindowedSize
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

    public WindowSize FullScreenSize
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
            UpdateDisplay(CurrentWindowSize, value);
        }
    }

    public bool IsUserResizingAllowed
    {
        get => _window.AllowUserResizing;
        set => _window.AllowUserResizing = value;
    }

    public WindowSize CurrentWindowSize
    {
        get => new WindowSize(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        set
        {
            WindowSize NewSize = ClampSize(value);
            if (NewSize != CurrentWindowSize)
            {
                UpdateDisplay(NewSize, IsFullScreen);
            }
        }
    }

    public WindowSize ScreenSize
    {
        get
        {
            DisplayMode Mode = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode;
            return new WindowSize(Mode.Width, Mode.Height);
        }
    }

    public event EventHandler<ScreenSizeChangeEventArgs>? ScreenSizeChange;


    // Private fields.
    private WindowSize _windowedSize;
    private WindowSize _fullScreenSize;
    private readonly GraphicsDeviceManager _graphics;
    private GameWindow _window;

    private Vector2 _virtualSize;
    private Vector2 _padding;


    // Constructors.
    internal GHDisplay(GraphicsDeviceManager manager, GameWindow window)
    {
        _graphics = manager ?? throw new ArgumentNullException(nameof(manager));
        _window = window ?? throw new ArgumentNullException(nameof(window));
        window.ClientSizeChanged += OnWindowSizeChangeEvent;

        WindowedSize = new WindowSize(ScreenSize.X / 2, ScreenSize.Y / 2);
        IsFullScreen = false;
        UpdateVirtualProperties();
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
        UpdateVirtualProperties();

        ScreenSizeChange?.Invoke(this, new ScreenSizeChangeEventArgs(CurrentWindowSize));
    }

    private void UpdateVirtualProperties()
    {

        float WindowAspectRatio = (float)CurrentWindowSize.X / (float)CurrentWindowSize.Y;
        if (WindowAspectRatio >= TargetAspectRatio)
        {
            _virtualSize = new Vector2(CurrentWindowSize.Y * TargetAspectRatio, CurrentWindowSize.Y);
        }
        else
        {
            _virtualSize = new Vector2(CurrentWindowSize.X, CurrentWindowSize.X / TargetAspectRatio);
        }
        _padding = new Vector2(CurrentWindowSize.X - _virtualSize.X, CurrentWindowSize.Y - _virtualSize.Y) * 0.5f;
    }

    private void UpdateDisplay(WindowSize size, bool isFullScreen)
    {
        _graphics.IsFullScreen = isFullScreen;
        _graphics.PreferredBackBufferWidth = size.X;
        _graphics.PreferredBackBufferHeight = size.Y;
        _graphics.ApplyChanges();
    }

    private WindowSize ClampSize(WindowSize size)
    {
        return new WindowSize(Math.Max(size.X, MIN_SIZE.X), Math.Max(size.Y, MIN_SIZE.Y));
    }

    // Inherited methods.
    public Vector2 ToNormalizedPosition(Vector2 windowPosition)
    {
        return (windowPosition - _padding) / _virtualSize;
    }

    public Vector2 ToWindowPosition(Vector2 normalizedPosition)
    {
        return (normalizedPosition * _virtualSize) + _padding;
    }

    public Vector2 ToNormalizedSize(Vector2 sizeInWindow)
    {
        return (sizeInWindow) / _virtualSize;
    }

    public Vector2 ToWindowSize(Vector2 normalizedSize)
    {
        return normalizedSize * _virtualSize;
    }
}