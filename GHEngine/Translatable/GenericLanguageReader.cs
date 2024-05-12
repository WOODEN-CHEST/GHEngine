using GHEngine.Translatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Translatable;

public class LanguageFileParser : ILanguageReader
{
    // Static fields.
    public const char KEY_VALUE_SEPARATOR = '=';


    // Constructors.
    public LanguageFileParser() { }


    // Private methods.
    private string FormatValue(string stringToFormat)
    {
        StringBuilder Builder = new(stringToFormat.Length);

        bool HadEscapeChar = false;
        for (int i = 0; i < stringToFormat.Length; i++)
        {
            if (!HadEscapeChar && stringToFormat[i] == '\\')
            {
                HadEscapeChar = !HadEscapeChar;
                continue;
            }

            if (HadEscapeChar)
            {
                Builder.Append(EscapedCharToChar(stringToFormat[i]));
            }
            else
            {
                Builder.Append(stringToFormat[i]);
            }
            HadEscapeChar = false;
        }

        return Builder.ToString();
    }

    private char EscapedCharToChar(char character)
    {
        return character switch
        {
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            '\\' => '\\',
            _ => character
        };
    }


    // Inherited methods.
    public void Read(ILanguage language, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        try
        {
            using FileStream Stream = File.OpenRead(path);
            Read(language, Stream);
        }
        catch (FileNotFoundException e)
        {
            throw new LanguageReadException($"Language file \"{path}\" not found. {e}");
        }
        catch (DirectoryNotFoundException e)
        {
            throw new LanguageReadException($"Directory for path \"{path}\" not found. {e}");
        }
        catch (PathTooLongException e)
        {
            throw new LanguageReadException($"Path \"{path}\" is too long. {e}");
        }
        catch (UnauthorizedAccessException e)
        {
            throw new LanguageReadException($"Not authorized to access file \"{path}\". {e}");
        }
        catch (IOException e)
        {
            throw new LanguageReadException($"IOException reading file \"{path}\". {e}");
        }
    }

    public void Read(ILanguage language, Stream dataStream)
    {
        StreamReader Reader = new(dataStream);
        string[] Data = Reader.ReadToEnd().Split('\n');

        for (int i = 0; i < Data.Length; i++)
        {
            int SplitIndex = Data[i].IndexOf(KEY_VALUE_SEPARATOR);
            if (SplitIndex == -1)
            {
                throw new LanguageReadException($"Missing value assignment in line {i}");
            }
            if (SplitIndex >= Data[i].Length - 1)
            {
                throw new LanguageReadException($"Missing value in line {i}");
            }
            string Key = Data[i].Substring(0, SplitIndex);
            if (string.IsNullOrEmpty(Key))
            {
                throw new LanguageReadException($"Invalid language text key: \"{Key}\"");
            }
            string Value = FormatValue(Data[i].Substring(SplitIndex + 1));
         
            language.AddText(Key, Value);
        }
    }
}