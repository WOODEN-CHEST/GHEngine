using GHEngine.Audio;
using GHEngine.Frame.Animation;
using GHEngine.Translatable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Wave;
using System.Collections;

namespace GHEngine.Assets;

public class GHAssetLoader : IAssetLoader
{
    // Fields.
    public string AssetPath
    {
        get => _assetPath;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _assetPath = FormatPath(value);
        }
    }


    // Private static fields.
    private const char WRONG_PATH_SEPARATOR = '\\';
    private const char CORRECT_PATH_SEPARATOR = '/';


    // Private fields.
    private string _assetPath;
    private readonly ContentManager _monogameContentManager;
    private readonly ILanguageReader _languageReader = new GHLanguageReader();
    private readonly WaveFormat _audioFormat;


    // Constructors.
    public GHAssetLoader(string assetPath, IServiceProvider serviceProvider, WaveFormat audioFormat)
    {
        AssetPath = assetPath;
        _monogameContentManager = new(serviceProvider, assetPath);
        _audioFormat = audioFormat ?? throw new ArgumentNullException(nameof(audioFormat));
    }


    // Methods.
    public string FormatPath(string path) => path.Replace(WRONG_PATH_SEPARATOR, CORRECT_PATH_SEPARATOR);

    public GHSpriteAnimation LoadSpriteAnimation(GHAnimationDefinition definition)
    {
        List<Texture2D> Frames = new();

        try
        {
            foreach (string FrameName in definition.Frames)
            {
                string CorrectedFrameName = FormatPath(FrameName);
                Texture2D Original = _monogameContentManager.Load<Texture2D>(CorrectedFrameName);
                Frames.Add(CreateTextureCopy(Original));
                _monogameContentManager.UnloadAsset(CorrectedFrameName);
            }
        }
        catch (ContentLoadException e)
        {
            throw new AssetLoadException(definition, e.ToString());
        }

        return new GHSpriteAnimation(definition.FPS, definition.Step, definition.IsLooped, definition.DrawRegion,
            definition.IsAnimated, Frames.ToArray());
    }

    public SpriteFont LoadFont(GHFontDefinition definition)
    {
        SpriteFont Original;
        string TargetPath = FormatPath(definition.Name);
        try
        {
            Original = _monogameContentManager.Load<SpriteFont>(TargetPath);
        }
        catch (ContentLoadException e)
        {
            throw new AssetLoadException(definition, e.ToString());
        }

        List<Rectangle > GlyphBounds = Original.Glyphs.Select(TargetGliyph => TargetGliyph.BoundsInTexture).ToList();
        List<Rectangle> Cropping = Original.Glyphs.Select(TargetGliyph => TargetGliyph.Cropping).ToList();

        SpriteFont Copy = new(CreateTextureCopy(Original.Texture), GlyphBounds, Cropping, Original.Characters.ToList(),
            Original.LineSpacing, Original.Spacing, Enumerable.Repeat(new Vector3(), GlyphBounds.Count).ToList(), 'h');

        _monogameContentManager.UnloadAsset(TargetPath);
        return Copy;
    }

    public Effect LoadShader(GHShaderDefinition definition)
    {
        SpriteEffect Original;
        string TargetPath = FormatPath(definition.Name);
        try
        {
            Original = _monogameContentManager.Load<SpriteEffect>(TargetPath);
        }
        catch (ContentLoadException e)
        {
            throw new AssetLoadException(definition, e.ToString());
        }

        Effect Copy = Original.Clone();
        _monogameContentManager.UnloadAsset(TargetPath);
        return Copy;
    }

    public ILanguage LoadLanguage(GHLanguageDefinition definition)
    {
        ILanguage Language = new GHLanguage(definition.LanguageNameLocal, definition.LanguageNameEnglish);
        _languageReader.Read(Language, Path.Combine(AssetPath, definition.Name));
        return Language;
    }

    public ISound LoadSound(GHSoundDefinition definition)
    {
        return LoadAudio(definition);
    }

    public ISound LoadSong(GHSongDefinition definition)
    {
        return LoadAudio(definition);
    }

    public Texture2D CreateTextureCopy(Texture2D texture)
    {
        Texture2D Copy = new(texture.GraphicsDevice, texture.Width, texture.Height, true, texture.Format);
        Color[] Data = new Color[texture.Width * texture.Height];
        Copy.SetData(Data);
        return Copy;
    }


    // Private methods.
    private ISound LoadAudio(AssetDefinition definition)
    {
        AudioFileReader Reader = null!;
        ISampleProvider Sampler;
        try
        {
            Reader = new(Path.Combine(AssetPath, definition.Name));
            {
                Sampler = Reader.ToSampleProvider();
            }

            if (!_audioFormat.Equals(Reader.WaveFormat))
            {
                throw new AssetLoadException(definition,
                    $"Wrong wave format: {Sampler.WaveFormat.Encoding} " +
                    $"(SampleRate:{Sampler.WaveFormat.SampleRate} Channels:{Sampler.WaveFormat.Channels})");
            }

            float[] Samples = new float[(int)Math.Ceiling(Reader.TotalTime.TotalSeconds
                * _audioFormat.Channels * _audioFormat.SampleRate)];
            Sampler.Read(Samples, 0, Samples.Length);
            Reader.Dispose();

            return new GHSound(Samples, _audioFormat);
        }
        catch (IOException e)
        {
            Reader?.Dispose();
            throw new AssetLoadException(definition, $"Couldn't read audio file. {e}");
        }
    }


    // Inherited methods.
    public object Load(AssetDefinition definition)
    {
        try
        {
            if (definition.Type == AssetType.Animation)
            {
                return LoadSpriteAnimation((GHAnimationDefinition)definition);
            }
            else if (definition.Type == AssetType.Song)
            {
                return LoadSong((GHSongDefinition)definition);
            }
            else if (definition.Type == AssetType.Sound)
            {
                return LoadSound((GHSoundDefinition)definition);
            }
            else if (definition.Type == AssetType.Font)
            {
                return LoadFont((GHFontDefinition)definition);
            }
            else if (definition.Type == AssetType.Shader)
            {
                return LoadShader((GHShaderDefinition)definition);
            }
            else if (definition.Type == AssetType.Language)
            {
                return LoadLanguage((GHLanguageDefinition)definition);
            }
            else
            {
                throw new AssetLoadException(definition, "Loader doesn't support loading this type of asset.");
            }
        }
        catch (InvalidCastException e)
        {
            throw new AssetLoadException(definition, $"Loader failed to identify definition type based on its asset type. {e}");
        }
    }

    public void Unload(object asset)
    {
        if (asset is ISpriteAnimation Animation)
        {
            foreach (Texture2D Texture in Animation.Frames)
            {
                Texture.Dispose();
            }
        }
        else if (asset is SpriteFont Font)
        {
            Font.Texture.Dispose();
        }
        else if (asset is ISound Sound)
        {
            Sound.Dispose();
        }
        else if (asset is ILanguage Language)
        {
            Language.Clear();
        }
        else if (asset is SpriteEffect Effect)
        {
            Effect.Dispose();
        }
        else
        {
            throw new AssetUnloadException(asset, "Loader does not support unloading this type of asset");
        }
    }
}