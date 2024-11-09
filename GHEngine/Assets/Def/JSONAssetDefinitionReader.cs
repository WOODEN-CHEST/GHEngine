using GHEngine.IO.JSON;
using GHEngine.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONAssetDefinitionReader : ISingleTypeAssetDefinitionReader
{
    // Private static fields.
    private const string KEY_NAME = "name";


    // Private fields.
    private readonly Dictionary<AssetType, JSONAssetDefinitionDeconstructor> _deconstructors = new();
    private readonly JSONDeserializer _deserializer = new();
    private readonly ILogger? _logger;


    // Constructors.
    public JSONAssetDefinitionReader(ILogger? logger = null)
    {
        _logger = logger;

        SetDeconstructor(AssetType.Animation, new JSONAnimationDeconstructor());
        SetDeconstructor(AssetType.Sound, new JSONSoundDeconstructor());
        SetDeconstructor(AssetType.Font, new JSONFontDeconstructor());
        SetDeconstructor(AssetType.Shader, new JSONShaderDeconstructor());
        SetDeconstructor(AssetType.Language, new JSONLanguageDeconstructor());
    }


    // Private methods.
    private void ReadJSONDefinitions(IAssetDefinitionCollection definitions, object json)
    {
        if (json is not JSONCompound Compound)
        {
            return;
        }

        foreach (KeyValuePair<string, object?> Entry in Compound)
        {
            if (Entry.Value is not JSONList AssetList)
            {
                continue;
            }

            if (_deconstructors.TryGetValue(new AssetType(Entry.Key, Entry.Key), 
                out JSONAssetDefinitionDeconstructor? Deconstructor))
            {
                ReadAssetDefinitionArray(definitions, AssetList, Deconstructor);
            }
        }
    }

    private void ReadAssetDefinitionArray(IAssetDefinitionCollection definitions,
        JSONList assetList,
        JSONAssetDefinitionDeconstructor deconstructor)
    {
        foreach (object? TargetObject in assetList)
        {
            if (TargetObject is not JSONCompound Compound)
            {
                _logger?.Warning($"Encountered non-compound in asset definition array: " +
                    $"{new JSONSerializer().Serialize(TargetObject, false)}");
                continue;
            }

            try
            {
                string Name = Compound.GetVerified<string>(KEY_NAME);
                definitions.Add(deconstructor.DeconstructDefinition(Name, Compound));
            }
            catch (JSONEntryException e)
            {
                _logger?.Error($"Failed to read asset definition. {e}");
            }
        }
    }


    // Methods.
    public void SetDeconstructor(AssetType type, JSONAssetDefinitionDeconstructor deconstructor)
    {
        _deconstructors[type] = deconstructor ?? throw new ArgumentNullException(nameof(deconstructor));
    }

    public void RemoveDeconstructor(AssetType type)
    {
        _deconstructors.Remove(type);
    }

    public void ClearDeconstructors()
    {
        _deconstructors.Clear();
    }

    public AssetType[] GetSupportedTypes()
    {
        return _deconstructors.Keys.ToArray();
    }


    // Inherited methods.
    public void Read(IAssetDefinitionCollection definitions, string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        foreach (string FilePath in Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories))
        {
            using (FileStream FileData = File.OpenRead(FilePath))
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
            object? JSONDefinitions = _deserializer.Deserialize(JSONData);
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