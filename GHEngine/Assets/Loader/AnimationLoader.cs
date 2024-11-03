using GHEngine.Assets.Def;
using GHEngine.Frame.Animation;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Assets.Loader;

public class AnimationLoader : GHStreamAssetLoader
{
    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;
    private readonly string[] _validExtensions = { ".png", ".jpg", ".gif", ".dds" };


    // Constructors.
    public AnimationLoader(IAssetStreamOpener streamOpener, GraphicsDevice graphicsDevice) : base(streamOpener)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
    }


    // Private methods.
    private string GetPathWithFileExtension(string modifiedPath)
    {
        foreach (string Extension in _validExtensions)
        {
            string PathWithExtension = Path.ChangeExtension(modifiedPath, Extension);
            if (StreamOpener.DoesFileExist(PathWithExtension))
            {
                return PathWithExtension;
            }
        }
        return modifiedPath;
    }

    private Stream GetStream(GHAnimationDefinition definition, AssetPath framePath)
    {
        AssetPath FullPath;
        string ModifiedPath = Path.Combine(definition.Type.RootPathName, framePath.Path);
        if (framePath.Type == AssetPathType.FileSystem)
        {
            FullPath = new(GetPathWithFileExtension(ModifiedPath), AssetPathType.FileSystem);
        }
        else
        {
            FullPath = new(ModifiedPath, framePath.Type);
        }

        return StreamOpener.GetStream(FullPath);
    }

    private GHSpriteAnimation LoadSpriteAnimation(GHAnimationDefinition definition)
    {
        List<Texture2D> Frames = new();

        try
        {
            foreach (AssetPath FrameName in definition.Frames)
            {
                Frames.Add(Texture2D.FromStream(_graphicsDevice, GetStream(definition, FrameName)));
            }
        }
        catch (Exception e)
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