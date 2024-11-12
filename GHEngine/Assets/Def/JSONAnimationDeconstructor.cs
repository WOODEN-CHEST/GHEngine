using GHEngine.IO.JSON;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONAnimationDeconstructor : JSONAssetDefinitionDeconstructor
{
    // Private static fields.
    private const string KEY_FRAMES = "frames";
    private const string KEY_FPS = "fps";
    private const string KEY_STEP = "step";
    private const string KEY_DRAW_REGION = "draw_region";
    private const string KEY_X = "x";
    private const string KEY_Y = "y";
    private const string KEY_WIDTH = "width";
    private const string KEY_HEIGHT = "height";
    private const string KEY_IS_LOOPED = "is_looped";
    private const string KEY_IS_ANIMATED = "is_animated";


    // Private fields.
    private readonly JSONPathDeconstructor _pathDeconstructor = new();


    // Private methods.
    private AssetPath[] GetAnimationFrames(JSONCompound animation)
    {
        JSONList Frames = animation.GetVerified<JSONList>(KEY_FRAMES);
        AssetPath[] FramePaths = new AssetPath[Frames.Count];
        for (int i = 0; i < Frames.Count; i++)
        {
            FramePaths[i] = _pathDeconstructor.GetPath(Frames.GetVerified<object>(i));
        }
        return FramePaths;
    }

    private float GetAsFloat(object targetObject, string entryName)
    {
        if (targetObject is long LongObj)
        {
            return (float)(long)targetObject;
        }
        if (targetObject is double DoubleObj)
        {
            return (float)DoubleObj;
        }
        throw new JSONEntryException($"Entry \"{entryName}\" is not a number.");
    }

    private RectangleF? GetDrawRegion(JSONCompound animation)
    {
        if (!animation.Get(KEY_DRAW_REGION, out JSONCompound? Compound))
        {
            return null;
        }

        float X = GetAsFloat(animation.GetVerified<object>(KEY_X), KEY_X);
        float Y = GetAsFloat(animation.GetVerified<object>(KEY_Y), KEY_Y);
        float Width = GetAsFloat(animation.GetVerified<object>(KEY_WIDTH), KEY_WIDTH);
        float Height = GetAsFloat(animation.GetVerified<object>(KEY_HEIGHT), KEY_HEIGHT);

        return new RectangleF(X, Y, Width, Height);
    }


    // Inherited methods.
    public override AssetDefinition DeconstructDefinition(string assetName, JSONCompound compound)
    {
        AssetPath[] Frames = GetAnimationFrames(compound);
        RectangleF? DrawRegion = GetDrawRegion(compound);
        double FPS = compound.GetVerifiedOrDefault(KEY_FPS, 60d);
        int Step = (int)compound.GetVerifiedOrDefault<long>(KEY_STEP, 1);
        bool IsLooped = compound.GetVerifiedOrDefault(KEY_IS_LOOPED, true);
        bool IsAnimated = compound.GetVerifiedOrDefault(KEY_IS_ANIMATED, true);

        return new GHAnimationDefinition(assetName, Frames, FPS, Step, DrawRegion, IsLooped, IsAnimated);
    }
}