using GHEngine.Assets.Def;
using GHEngine.Frame.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
    private Texture2D LoadTexture(Stream stream)
    {
        try
        {
            // Such a inefficient way to do it.g
            using Image<Rgba32> LoadedImage = Image.Load<Rgba32>(stream);

            Texture2D Texture = new Texture2D(_graphicsDevice, LoadedImage.Width, LoadedImage.Height, false, SurfaceFormat.Color);
            IEnumerable<Rgba32> RgbaPixels = LoadedImage.GetPixelMemoryGroup().SelectMany(memory => memory.ToArray());
            Microsoft.Xna.Framework.Color[] MonoGamePixels = RgbaPixels.Select(
                pixel => new Microsoft.Xna.Framework.Color(pixel.R, pixel.G, pixel.B, pixel.A)).ToArray();
            Texture.SetData(MonoGamePixels);

            return Texture;
        }
        catch (Exception e)
        {
            throw new AssetLoadException("Exception while loading image.", e);
        };
    }

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
                Frames.Add(LoadTexture(GetStream(definition, FrameName)));
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
    public override IDisposable Load(AssetDefinition definition)
    {
        if (definition is not GHAnimationDefinition AnimationDefinition)
        {
            throw new AssetLoadException("Asset definition is not an animation definition.");
        }

        return LoadSpriteAnimation(AnimationDefinition);
    }
}