using GHEngine.IO.JSON;
using GHEngine.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHAssetDefinitionReader : IAssetDefinitionReader
{
    // Private static fields.
    private const string KEY_NAME = "name";

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

    private const string KEY_NAME_LOCAL = "name_local";
    private const string KEY_NAME_ENGLISH = "name_english";


    // Private fields.
    private JSONDeserializer _deserializer = new();
    private ILogger? _logger;


    // Constructors.
    public GHAssetDefinitionReader(ILogger? logger)
    {
        _logger = logger;
    }


    // Private methods.
    private void ReadJSONDefinitions(IAssetDefinitionCollection definitions, JSONObject json)
    {
        if (json is not JSONCompound Compound)
        {
            return;
        }

        ReadAssetDefinitionArray(definitions, Compound, AssetType.Animation.RootPathName, ReadAnimation);
        ReadAssetDefinitionArray(definitions, Compound, AssetType.Font.RootPathName, ReadFont);
        ReadAssetDefinitionArray(definitions, Compound, AssetType.Shader.RootPathName, ReadShader);
        ReadAssetDefinitionArray(definitions, Compound, AssetType.Song.RootPathName, ReadSong);
        ReadAssetDefinitionArray(definitions, Compound, AssetType.Sound.RootPathName, ReadSound);
        ReadAssetDefinitionArray(definitions, Compound, AssetType.Language.RootPathName, ReadLanguage);
    }

    private void ReadAssetDefinitionArray(IAssetDefinitionCollection definitions,
        JSONCompound compound,
        string assetTypeName,
        Func<string, JSONCompound, AssetDefinition> executor)
    {
        JSONArray? Assets = compound.GetOptionalVerifiedEntry<JSONArray>(assetTypeName);
        if (Assets == null)
        {
            return;
        }

        foreach (JSONObject? TargetObject in Assets)
        {
            if (TargetObject is not JSONCompound Compound)
            {
                _logger?.Warning("Encountered non-compound in asset definition array");
                continue;
            }

            try
            {
                string Name = Compound.GetVerifiedEntry<JSONString>(KEY_NAME);
                definitions.Add(executor.Invoke(Name, Compound));
            }
            catch (JSONEntryException e)
            {
                _logger?.Error($"Failed to read asset definition. {e}");
            }
        }
    }

    private AssetDefinition ReadAnimation(string name, JSONCompound animation)
    {
        string[] Frames = GetAnimationFrames(animation);
        Rectangle? DrawRegion = GetDrawRegion(animation);
        double FPS = animation.GetOptionalVerifiedEntry<JSONRealNumber>(KEY_FPS) ?? 60d;
        int Step = animation.GetOptionalVerifiedEntry<JSONInteger>(KEY_STEP) ?? 1;
        bool IsLooped = animation.GetOptionalVerifiedEntry<JSONBoolean>(KEY_IS_LOOPED) ?? true;
        bool IsAnimated = animation.GetOptionalVerifiedEntry<JSONBoolean>(KEY_IS_ANIMATED) ?? true;

        return new GHAnimationDefinition(name, Frames, FPS, Step, DrawRegion, IsLooped, IsAnimated);
    }

    private string[] GetAnimationFrames(JSONCompound animation)
    {
        JSONArray Frames = animation.GetVerifiedEntry<JSONArray>(KEY_FRAMES);
        string[] FramePaths = new string[Frames.Count];
        for (int i = 0; i < Frames.Count; i++)
        {
            if (Frames[i] is not JSONString StringObject)
            {
                throw new JSONEntryException("Animation frame is not a string.");
            }
            FramePaths[i] = StringObject;
        }
        return FramePaths;
    }

    private Rectangle? GetDrawRegion(JSONCompound animation)
    {
        JSONCompound? DrawRegionCompound = animation.GetOptionalVerifiedEntry<JSONCompound>(KEY_DRAW_REGION);
        if (DrawRegionCompound == null)
        {
            return null;
        }

        int X = DrawRegionCompound.GetVerifiedEntry<JSONInteger>(KEY_X);
        int Y = DrawRegionCompound.GetVerifiedEntry<JSONInteger>(KEY_Y);
        int Width = DrawRegionCompound.GetVerifiedEntry<JSONInteger>(KEY_WIDTH);
        int Height = DrawRegionCompound.GetVerifiedEntry<JSONInteger>(KEY_HEIGHT);

        return new Rectangle(X, Y, Width, Height);
    }

    private AssetDefinition ReadSound(string name, JSONCompound soundCompound) => new GHSoundDefinition(name);

    private AssetDefinition ReadSong(string name, JSONCompound soundCompound) => new GHSongDefinition(name);

    private AssetDefinition ReadFont(string name, JSONCompound soundCompound) => new GHFontDefinition(name);

    private AssetDefinition ReadShader(string name, JSONCompound soundCompound) => new GHShaderDefinition(name);

    private AssetDefinition ReadLanguage(string name, JSONCompound soundCompound)
    {
        string NameLocal = soundCompound.GetVerifiedEntry<JSONString>(KEY_NAME_LOCAL);
        string NameEnglish = soundCompound.GetVerifiedEntry<JSONString>(KEY_NAME_ENGLISH);
        return new GHLanguageDefinition(name, NameEnglish, NameLocal);
    }


    // Inherited methods.
    public void Read(IAssetDefinitionCollection definitions, string directoryPath)
    {
        foreach (string FielPath in Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories))
        {
            using (FileStream FileData = File.OpenRead(directoryPath))
            {
                Read(definitions, FileData);
            }
        }
    }

    public void Read(IAssetDefinitionCollection definitions, Stream dataStream)
    {
        string JSONData = new StreamReader(dataStream).ReadToEnd();

        try
        {
            JSONObject? JSONDefinitions = _deserializer.Deserialize(JSONData);
            if (JSONDefinitions != null)
            {
                ReadJSONDefinitions(definitions, JSONDefinitions);
            }
        }
        catch (JSONDeserializeException e)
        {
            throw new AssetDefinitionReadException($"Malformed JSON for asset definition: {e}");
        }
    }
}