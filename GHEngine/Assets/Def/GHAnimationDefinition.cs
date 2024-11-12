using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class GHAnimationDefinition : AssetDefinition
{
    // Fields.
    public AssetPath[] Frames => _frames.ToArray();
    public double FPS { get; private init; }
    public int Step { get; private init; }
    public RectangleF? DrawRegion { get; private init; }
    public bool IsLooped { get; private init; }
    public bool IsAnimated { get; private init; }


    // Private fields.
    private AssetPath[] _frames;


    // Constructors. 
    public GHAnimationDefinition(string name,
        AssetPath[] frames,
        double fps,
        int step,
        RectangleF? drawRegion,
        bool isLooped,
        bool isAnimated)
        : base(AssetType.Animation, name)
    {
        _frames = frames?.ToArray() ?? throw new ArgumentNullException(nameof(frames));
        FPS = fps;
        Step = step;
        DrawRegion = drawRegion;
        IsLooped = isLooped;
        IsAnimated = isAnimated;
    }
}