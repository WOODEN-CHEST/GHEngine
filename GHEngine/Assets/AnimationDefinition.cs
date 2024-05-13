using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class AnimationDefinition : AssetDefinition
{
    // Fields.
    public string[] Frames => _frames.ToArray();
    public double FPS { get; private init; }
    public int Step { get; private init; }


    // Private fields.
    private string[] _frames;


    // Constructors. 
    public AnimationDefinition(string name, string[] frames, double fps = 60d, int step = 1)
        : base(AssetType.Animation, name)
    {
        _frames = frames ?? throw new ArgumentNullException(nameof(frames));
        FPS = fps;
        Step = step;
    }
}