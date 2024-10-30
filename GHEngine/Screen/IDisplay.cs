﻿using Microsoft.Xna.Framework;
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
    IntVector WindowedSize { get; set; }
    IntVector FullScreenSize { get; set; }
    IntVector CurrentWindowSize { get; set; }
    IntVector ScreenSize { get; }
    bool IsFullScreen { get; set; }
    bool IsUserResizingAllowed { get; set; }

    event EventHandler<ScreenSizeChangeEventArgs>? ScreenSizeChange;


    // Methods.
    Vector2 ToNormalizedPosition(Vector2 windowPosition);

    Vector2 ToWindowPosition(Vector2 normalizedPosition);

    Vector2 ToNormalizedSize(Vector2 windowSize);

    Vector2 ToWindowSize(Vector2 normalizedSize);
}