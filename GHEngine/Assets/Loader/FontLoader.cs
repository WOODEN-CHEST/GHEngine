using GHEngine.Assets.Def;
using GHEngine.GameFont;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Loader;

public class FontLoader : GHStreamAssetLoader
{
    // Constructors,
    public FontLoader(IAssetStreamOpener streamOpener) : base(streamOpener) { }


    // Private fields.
    private readonly string[] _validExtensions = { ".ttf", ".otf" };
    private readonly GraphicsDevice _graphicsDevice;


    // Constructors.
    public FontLoader(IAssetStreamOpener streamOpener, GraphicsDevice graphicsDevice) : base(streamOpener)
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


    // Inherited methods.
    public override IDisposable Load(AssetDefinition definition)
    {
        if (definition is not GHFontDefinition FontDefinition)
        {
            throw new AssetLoadException("Asset definition is not a font definition.");
        }

        FontCollection UselessCollection = new();

        string FullPath = Path.Combine(FontDefinition.Type.RootPathName, FontDefinition.TargetPath.Path);
        if (FontDefinition.TargetPath.Type == AssetPathType.FileSystem)
        {
            FullPath = GetPathWithFileExtension(FullPath);
        }

        FontFamily Family;
        try
        {
            Family = UselessCollection.Add(StreamOpener.GetStream(
                new AssetPath(FullPath, FontDefinition.TargetPath.Type)));
        }
        catch (Exception e)
        {
            throw new AssetLoadException("Failed to load font.", e);
        }

        return new GHFontFamily(_graphicsDevice, Family);
    }
}