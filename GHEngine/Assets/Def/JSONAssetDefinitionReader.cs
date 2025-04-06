using GHEngine.IO.JSON;
using GHEngine.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONAssetDefinitionReader : IAssetDefinitionConverter
{
    // Private static fields.
    private const string KEY_NAME = "name";


    // Private fields.
    private readonly Dictionary<AssetType, JSONAssetDefinitionConverter> _converters = new();
    private readonly JSONDeserializer _deserializer = new();
    private readonly JSONSerializer _serializer = new();
    private readonly ILogger? _logger;


    // Constructors.
    public JSONAssetDefinitionReader(ILogger? logger = null)
    {
        _logger = logger;

        SetDeconstructor(AssetType.Animation, new JSONAnimationConverter());
        SetDeconstructor(AssetType.Sound, new JSONSoundConverter());
        SetDeconstructor(AssetType.Font, new JSONFontConverter());
        SetDeconstructor(AssetType.Shader, new JSONShaderConverter());
        SetDeconstructor(AssetType.Language, new JSONLanguageConverter());
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

            if (_converters.TryGetValue(new AssetType(Entry.Key, Entry.Key), 
                out JSONAssetDefinitionConverter? Deconstructor))
            {
                ReadAssetDefinitionArray(definitions, AssetList, Deconstructor);
            }
        }
    }

    private void ReadAssetDefinitionArray(IAssetDefinitionCollection definitions,
        JSONList assetList,
        JSONAssetDefinitionConverter deconstructor)
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
                definitions.Add(deconstructor.ReadDefinition(Name, Compound));
            }
            catch (JSONEntryException e)
            {
                _logger?.Error($"Failed to read asset definition. {e}");
            }
        }
    }

    private JSONList GetAssetsOfType(AssetType type, IAssetDefinitionCollection definitions)
    {
        JSONList AssetList = new();
        if (!_converters.TryGetValue(type, out var Converter))
        {
            return AssetList;
        }
        
        foreach (AssetDefinition Definition in definitions.GetOfType(type))
        {
            JSONCompound Compound = new();
            Compound.Add(KEY_NAME, Definition.Name);
            Converter.WriteDefinition(Definition, Compound);
            AssetList.Add(Compound);
        }
        return AssetList;
    }


    // Methods.
    public void SetDeconstructor(AssetType type, JSONAssetDefinitionConverter deconstructor)
    {
        _converters[type] = deconstructor ?? throw new ArgumentNullException(nameof(deconstructor));
    }

    public void RemoveDeconstructor(AssetType type)
    {
        _converters.Remove(type);
    }

    public void ClearDeconstructors()
    {
        _converters.Clear();
    }

    public AssetType[] GetSupportedTypes()
    {
        return _converters.Keys.ToArray();
    }


    // Inherited methods.
    public void Read(IAssetDefinitionCollection definitions, string directoryPath)
    {
        ArgumentNullException.ThrowIfNull(definitions, nameof(definitions));
        ArgumentNullException.ThrowIfNull(directoryPath, nameof(directoryPath));

        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        try
        {
            foreach (string FilePath in Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories))
            {
                using (FileStream FileData = File.OpenRead(FilePath))
                {
                    Read(definitions, FileData);
                }
            }
        }
        catch (IOException e)
        {
            throw new AssetDefinitionReadException($"Failed to read asset definitions in directory: {e}");
        }
    }

    public void Read(IAssetDefinitionCollection definitions, Stream dataStream)
    {
        ArgumentNullException.ThrowIfNull(definitions, nameof(definitions));
        ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));

        string JSONData = new StreamReader(dataStream).ReadToEnd();

        try
        {
            object? JSONDefinitions = _deserializer.Deserialize(JSONData);
            if (JSONDefinitions != null)
            {
                ReadJSONDefinitions(definitions, JSONDefinitions);
            }
        }
        catch (Exception e) when ((e is IOException) || (e is JSONEntryException))
        {
            throw new AssetDefinitionReadException($"Malformed JSON for asset definition: {e}");
        }
    }

    public void Write(IAssetDefinitionCollection definitions, string assetFilePath)
    {
        if (!Path.IsPathFullyQualified(assetFilePath))
        {
            throw new ArgumentException("Asset definition write path must be fully qualified.");
        }

        string? ParentDir = Path.GetDirectoryName(assetFilePath);
        string ModifiedPath = Path.ChangeExtension(assetFilePath, ".json");

        try
        {
            if (ParentDir != null)
            {
                Directory.CreateDirectory(ParentDir);
            }
            
            if (File.Exists(ModifiedPath))
            {
                throw new AssetDefinitionReadException($"Cannot write asset definitions to file \"{ModifiedPath}\" " +
                    $"since it already exists.");
            }
            Write(definitions, File.OpenWrite(ModifiedPath));
        }
        catch (IOException e)
        {
            throw new AssetDefinitionWriteException($"Couldn't write asset definitions to file \"{assetFilePath}\": {e}");
        }
    }

    public void Write(IAssetDefinitionCollection definitions, Stream dataStream)
    {
        JSONCompound RootCompound = new();

        AssetType[] AssetTypes = new AssetType[] {
            AssetType.Animation,
            AssetType.Sound,
            AssetType.Font,
            AssetType.Language,
            AssetType.Shader,
        };

        foreach (AssetType Type in AssetTypes)
        {
            RootCompound.Add(Type.TypeName, GetAssetsOfType(Type, definitions));
        }

        try
        {
            new StreamWriter(dataStream, Encoding.UTF8).Write(_serializer.Serialize(RootCompound, true));
        }
        catch (IOException e)
        {
            throw new AssetDefinitionWriteException($"Couldn't write asset definitions: {e}");
        }
    }
}