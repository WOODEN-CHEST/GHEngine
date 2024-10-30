using GHEngine.IO.JSON;
using GHEngine.Translatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Translatable;

public class JSONLanguageReader : ILanguageReader
{
    // Constructors.
    public JSONLanguageReader() { }


    // Inherited methods.
    public void Read(ILanguage language, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        using FileStream Stream = File.OpenRead(path);
        Read(language, Stream);
    }

    public void Read(ILanguage language, Stream dataStream)
    {
        StreamReader Reader = new(dataStream);

        object? ReadObject = new JSONDeserializer().Deserialize(Reader.ReadToEnd());
        if (ReadObject is not JSONCompound Compound)
        {
            throw new LanguageReadException("Invalid language data, root is not a JSON compound");
        }

        foreach (KeyValuePair<string, object?> Translation in Compound)
        {
            if (Translation.Value is not string Value)
            {
                continue;
            }
            language.AddText(Translation.Key, Value);
        }
    }
}