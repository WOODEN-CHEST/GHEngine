using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GHEngine.IO;

public interface IUserInput
{
    // Fields.
    int MouseScrollChangeAmount { get; }
    int KeysDownCountCurrent { get; }
    int KeysDownCountPrevious { get; }
    Vector2 VirtualMousePositionCurrent { get; }
    Vector2 VirtualMousePositionPrevious { get; }
    Vector2 ActualMousePositionCurrent { get; set; }
    Vector2 ActualMousePositionPrevious { get; }
    int MouseButtonsPressedCountCurrent { get; }
    int MouseButtonsPressedCountPrevious { get; }
    bool IsWindowFocused { get; }
    bool IsAltF4Allowed { get; set; }
    bool IsMouseVisible { get; set; }
    MouseCursor CurrentCursor { set; }
    Vector2 InputAreaSizePixels { get; set; }


    event EventHandler<TextInputEventArgs>? TextInput;
    event EventHandler<FileDropEventArgs>? FileDrop;


    // Methods.
    void RefreshInput();
    bool AreKeysDown(params Keys[] keys);
    bool AreKeysUp(params Keys[] keys);
    bool WereKeysJustPressed(params Keys[] keys);
    bool WereKeysJustReleased(params Keys[] keys);
    bool WereKeysDown(params Keys[] keys);
    bool WereKeysUp(params Keys[] keys);
    bool AreMouseButtonsDown(params MouseButton[] buttons);
    bool AreMouseButtonsUp(params MouseButton[] buttons);
    bool WereMouseButtonsDown(params MouseButton[] buttons);
    bool WereMouseButtonsUp(params MouseButton[] buttons);
    bool WereMouseButtonsJustPressed(params MouseButton[] buttons);
    bool WereMouseButtonsJustReleased(params MouseButton[] buttons);
}