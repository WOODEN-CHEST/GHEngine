using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Screen;

public interface IDisplay
{
    // Static fields.
    public const float ASPECT_RATIO_SQUARE = 1f; 
    public const float ASPECT_RATIO_FULLSCREEN = 4f / 3f; 
    public const float ASPECT_RATIO_STANDART = 16f / 9f; 
    public const float ASPECT_RATIO_ULTRAWIDE = 21f / 9f; 


    // Fields.
    float TargetAspectRatio { get; set; }
    WindowSize WindowedSize { get; set; }
    WindowSize FullScreenSize { get; set; }
    WindowSize CurrentWindowSize { get; set; }
    WindowSize ScreenSize { get; }
    bool IsFullScreen { get; set; }
    bool IsUserResizingAllowed { get; set; }

    event EventHandler<ScreenSizeChangeEventArgs>? ScreenSizeChange;


    // Methods.
    Vector2 ToNormalizedPosition(Vector2 windowPosition);

    Vector2 ToWindowPosition(Vector2 normalizedPosition);
}