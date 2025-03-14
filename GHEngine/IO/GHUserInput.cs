using GHEngine.Collections;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GHEngine.IO;

public class GHUserInput : IUserInput
{
    // Fields.
    public int MouseScrollChangeAmount { get; private set; }
    public int KeysDownCountCurrent => _keysDownCount.Current;
    public int KeysDownCountPrevious => _keysDownCount.Previous;
    public Vector2 VirtualMousePositionCurrent => _virtualMousePosition.Current;
    public Vector2 VirtualMousePositionPrevious => _virtualMousePosition.Previous;

    public Vector2 ActualMousePositionCurrent
    {
        get => _actualMousePosition.Current;
        set
        {
            Mouse.SetPosition((int)value.X, (int)value.Y);
            _mouseState.SetValue(Mouse.GetState());
        }
    }

    public Vector2 ActualMousePositionPrevious => _actualMousePosition.Previous;
    public int MouseButtonsPressedCountCurrent => _mouseButtonsPressedCount.Current;
    public int MouseButtonsPressedCountPrevious => _mouseButtonsPressedCount.Previous;
    public bool IsWindowFocused => _game.IsActive;

    public bool IsAltF4Allowed
    {
        get => _window.AllowAltF4;
        set => _window.AllowAltF4 = value;
    }

    public bool IsMouseVisible
    {
        get => _game.IsMouseVisible;
        set => _game.IsMouseVisible = value;
    }

    public MouseCursor CurrentCursor
    {
        set => Mouse.SetCursor(value);
    }

    public Vector2 InputAreaSizePixels
    {
        get => _inputAreaSizePixels;
        set
        {
            _inputAreaSizePixels = value;
            InputAreaRatio = value.X / value.Y;
        }
    }

    public float InputAreaRatio { get; set; }

    public event EventHandler<TextInputEventArgs>? TextInput;
    public event EventHandler<FileDropEventArgs>? FileDrop;


    // Private fields.
    private Vector2 _inputAreaSizePixels;

    public readonly DeltaValue<KeyboardState> _keyboardState = new();
    public readonly DeltaValue<int> _keysDownCount = new();
    public readonly DeltaValue<MouseState> _mouseState = new();
    public readonly DeltaValue<Vector2> _virtualMousePosition = new();
    public readonly DeltaValue<Vector2> _actualMousePosition = new();
    public readonly DeltaValue<int> _mouseButtonsPressedCount = new();

    private readonly GameWindow _window;
    private readonly Game _game;


    // Constructors.
    public GHUserInput(GameWindow window, Game game)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _window.TextInput += OnTextInputEvent;
        _window.FileDrop += OnFileDropEvent;
    }



    // Private methods.
    private void OnTextInputEvent(object? sender, TextInputEventArgs args)
    {
        TextInput?.Invoke(this, args);
    }

    private void OnFileDropEvent(object? sender, FileDropEventArgs args)
    {
        FileDrop?.Invoke(this, args);
    }

    private void UpdateKeyboardInfo()
    {
        KeyboardState NewState = Keyboard.GetState();
        _keyboardState.SetValue(NewState);
        _keysDownCount.SetValue(_keyboardState.Current.GetPressedKeyCount());
    }

    private void UpdateMouseInfo()
    {
        _mouseState.SetValue(Mouse.GetState());
        _actualMousePosition.SetValue(_mouseState.Current.Position.ToVector2());
        _virtualMousePosition.SetValue(_mouseState.Current.Position.ToVector2()
            / new Vector2(_window.ClientBounds.Width, _window.ClientBounds.Height));

        int MouseButtonsPressed = 0;
        if (_mouseState.Current.LeftButton == ButtonState.Pressed) MouseButtonsPressed++;
        if (_mouseState.Current.MiddleButton == ButtonState.Pressed) MouseButtonsPressed++;
        if (_mouseState.Current.RightButton == ButtonState.Pressed) MouseButtonsPressed++;
        _mouseButtonsPressedCount.SetValue(MouseButtonsPressed);
    }

    private bool AreKeysInState(Keys[] keys, KeyState keyState, KeyboardState keyboardState)
    {
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        bool IsDown = keyState == KeyState.Down;

        foreach (Keys Key in keys)
        {
            if (keyboardState.IsKeyDown(Key) != IsDown)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsMouseButtonPressed(MouseButton button, MouseState state)
    {
        return button switch
        {
            MouseButton.Left => state.LeftButton == ButtonState.Pressed,
            MouseButton.Middle => state.MiddleButton == ButtonState.Pressed,
            MouseButton.Right => state.RightButton == ButtonState.Pressed,
            _ => throw new EnumValueException(nameof(button), button)
        };
    }

    private bool AreButtonsInState(MouseButton[] buttons, ButtonState buttonState, MouseState mouseState)
    {
        if(buttons == null)
        {
            throw new ArgumentNullException(nameof(buttons));
        }
        bool IsPressed = buttonState == ButtonState.Pressed;

        foreach (MouseButton Button in buttons)
        {
            if (IsMouseButtonPressed(Button, mouseState) != IsPressed)
            {
                return false;
            }
        }
        return true;
    }



    // Inherited methods.
    public void RefreshInput()
    {
        UpdateKeyboardInfo();
        UpdateMouseInfo();
    }

    public bool AreKeysDown(params Keys[] keys)
    {
        return AreKeysInState(keys, KeyState.Down, _keyboardState.Current);
    }

    public bool AreKeysUp(params Keys[] keys)
    {
        return AreKeysInState(keys, KeyState.Up, _keyboardState.Current);
    }

    public bool WereKeysJustPressed(params Keys[] keys)
    {
        return AreKeysInState(keys, KeyState.Up, _keyboardState.Previous)
            && AreKeysInState(keys, KeyState.Down, _keyboardState.Current);
    }

    public bool WereKeysJustReleased(params Keys[] keys)
    {
        return AreKeysInState(keys, KeyState.Up, _keyboardState.Current)
            && AreKeysInState(keys, KeyState.Down, _keyboardState.Previous);
    }

    public bool WereKeysDown(params Keys[] keys)
    {
        return AreKeysInState(keys, KeyState.Down, _keyboardState.Previous);
    }

    public bool WereKeysUp(params Keys[] keys)
    {
        return AreKeysInState(keys, KeyState.Up, _keyboardState.Previous);
    }

    public bool AreMouseButtonsDown(params MouseButton[] buttons)
    {
        return AreButtonsInState(buttons, ButtonState.Pressed, _mouseState.Current);
    }

    public bool AreMouseButtonsUp(params MouseButton[] buttons)
    {
        return AreButtonsInState(buttons, ButtonState.Released, _mouseState.Current);
    }

    public bool WereMouseButtonsDown(params MouseButton[] buttons)
    {
        return AreButtonsInState(buttons, ButtonState.Pressed, _mouseState.Previous);
    }

    public bool WereMouseButtonsUp(params MouseButton[] buttons)
    {
        return AreButtonsInState(buttons, ButtonState.Released, _mouseState.Previous);
    }

    public bool WereMouseButtonsJustPressed(params MouseButton[] buttons)
    {
        return AreButtonsInState(buttons, ButtonState.Pressed, _mouseState.Current)
            && AreButtonsInState(buttons, ButtonState.Released, _mouseState.Previous);
    }

    public bool WereMouseButtonsJustReleased(params MouseButton[] buttons)
    {
        return AreButtonsInState(buttons, ButtonState.Pressed, _mouseState.Previous)
            && AreButtonsInState(buttons, ButtonState.Released, _mouseState.Current);
    }
}