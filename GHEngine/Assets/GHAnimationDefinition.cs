using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHAnimationDefinition : AssetDefinition
{
    // Fields.
    public string[] Frames => _frames.ToArray();
    public double FPS { get; private init; }
    public int Step { get; private init; }
    public Rectangle? DrawRegion { get; private init; }
    public bool IsLooped { get; private init; }
    public bool IsAnimated { get; private init; }


    // Private fields.
    private string[] _frames;


    // Constructors. 
    public GHAnimationDefinition(string name, 
        string[] frames,
        double fps = 60d,
        int step = 1,
        Rectangle? drawRegion = null,
        bool isLooped = true,
        bool isAnimated = true)
        : base(AssetType.Animation, name)
    {
        _frames = frames ?? throw new ArgumentNullException(nameof(frames));
        FPS = fps;
        Step = step;
        DrawRegion = drawRegion;
        IsLooped = isLooped;
        IsAnimated = isAnimated;
    }
}