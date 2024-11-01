using GHEngine.Assets.Def;
using GHEngine.Frame.Animation;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Assets.Loader;

public class AnimationLoader : GHStreamAssetLoader
{
    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;


    // Constructors.
    public AnimationLoader(IAssetStreamOpener streamOpener, GraphicsDevice graphicsDevice) : base(streamOpener)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
    }


    // Private methods.
    private GHSpriteAnimation LoadSpriteAnimation(GHAnimationDefinition definition)
    {
        List<Texture2D> Frames = new();

        try
        {
            foreach (AssetPath FrameName in definition.Frames)
            {
                AssetPath FullPath = new(Path.Combine(definition.Type.RootPathName, FrameName.Path), FrameName.Type);
                Frames.Add(Texture2D.FromStream(_graphicsDevice, StreamOpener.GetStream(FullPath)));
            }
        }
        catch (ContentLoadException e)
        {
            throw new AssetLoadException(definition, e.ToString());
        }

        return new GHSpriteAnimation(definition.FPS, definition.Step, definition.IsLooped, 
            definition.DrawRegion, definition.IsAnimated, Frames.ToArray());
    }


    // Inherited methods.
    public override object Load(AssetDefinition definition)
    {
        if (definition is not GHAnimationDefinition AnimationDefinition)
        {
            throw new AssetLoadException("Asset definition is not an animation definition.");
        }

        return LoadSpriteAnimation(AnimationDefinition);
    }
}