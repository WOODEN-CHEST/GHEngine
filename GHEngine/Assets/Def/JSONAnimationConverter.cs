using GHEngine.IO.JSON;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONAnimationConverter : JSONAssetDefinitionConverter
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
    private readonly JSONPathConverter _pathConverter = new();


    // Private methods.
    private AssetPath[] GetAnimationFrames(JSONCompound animation)
    {
        JSONList Frames = animation.GetVerified<JSONList>(KEY_FRAMES);
        AssetPath[] FramePaths = new AssetPath[Frames.Count];
        for (int i = 0; i < Frames.Count; i++)
        {
            FramePaths[i] = _pathConverter.GetPath(Frames.GetVerified<object>(i));
        }
        return FramePaths;
    }

    private RectangleF? GetDrawRegion(JSONCompound animation)
    {
        if (!animation.Get(KEY_DRAW_REGION, out JSONCompound? Compound))
        {
            return null;
        }

        float X = (float)JSONUtilities.GetDouble(animation.GetVerified<object>(KEY_X));
        float Y = (float)JSONUtilities.GetDouble(animation.GetVerified<object>(KEY_Y));
        float Width = (float)JSONUtilities.GetDouble(animation.GetVerified<object>(KEY_WIDTH));
        float Height = (float)JSONUtilities.GetDouble(animation.GetVerified<object>(KEY_HEIGHT));

        return new RectangleF(X, Y, Width, Height);
    }

    private JSONCompound CreateDrawRegion(RectangleF drawRegion)
    {
        JSONCompound Compound = new();

        Compound.Add(KEY_X, (double)drawRegion.X);
        Compound.Add(KEY_Y, (double)drawRegion.Y);
        Compound.Add(KEY_WIDTH, (double)drawRegion.Width);
        Compound.Add(KEY_HEIGHT, (double)drawRegion.Height);

        return Compound;
    }

    private JSONList CreateAnimationFrames(AssetPath[] paths)
    {
        JSONList List = new JSONList();

        foreach (AssetPath TargetPath in paths)
        {
            List.Add(_pathConverter.WritePath(TargetPath));
        }

        return List;
    }


    // Inherited methods.
    public override AssetDefinition ReadDefinition(string assetName, JSONCompound compound)
    {
        AssetPath[] Frames = GetAnimationFrames(compound);
        RectangleF? DrawRegion = GetDrawRegion(compound);
        double FPS = compound.GetVerifiedOrDefault(KEY_FPS, 60d);
        int Step = (int)compound.GetVerifiedOrDefault<long>(KEY_STEP, 1);
        bool IsLooped = compound.GetVerifiedOrDefault(KEY_IS_LOOPED, true);
        bool IsAnimated = compound.GetVerifiedOrDefault(KEY_IS_ANIMATED, true);

        return new GHAnimationDefinition(assetName, Frames, FPS, Step, DrawRegion, IsLooped, IsAnimated);
    }

    public override void WriteDefinition(AssetDefinition definition, JSONCompound compound)
    {
        GHAnimationDefinition CastDefinition = (GHAnimationDefinition)definition;

        compound.Add(KEY_FRAMES, CreateAnimationFrames(CastDefinition.Frames));
        compound.Add(KEY_FPS, CastDefinition.FPS);
        compound.Add(KEY_STEP, (long)CastDefinition.Step);

        if (CastDefinition.DrawRegion.HasValue)
        {
            compound.Add(KEY_DRAW_REGION, CreateDrawRegion(CastDefinition.DrawRegion.Value));
        }
        
        compound.Add(KEY_IS_LOOPED, CastDefinition.IsLooped);
        compound.Add(KEY_IS_ANIMATED, CastDefinition.IsAnimated);
    }
}