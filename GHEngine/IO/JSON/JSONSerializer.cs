﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONSerializer
{
    // Methods.
    public string Serialize(JSONObject jsonObject, bool format)
    {
        return SerializeObject(jsonObject, format, 0);
    }


    // Private methods.
    private string SerializeObject(JSONObject? jsonObject, bool format, int indentLevel)
    {
        if (jsonObject == null)
        {
            return "null";
        }
        else if (jsonObject is JSONString StringObject)
        {
            return $"\"{FormatString(StringObject.Value)}\"";
        }
        else if (jsonObject is JSONInteger IntegerObject)
        {
            return IntegerObject.Value.ToString();
        }
        else if (jsonObject is JSONRealNumber RealNumberObject)
        {
            return RealNumberObject.Value.ToString(CultureInfo.InvariantCulture);
        }
        else if (jsonObject is JSONBoolean BooleanObject)
        {
            return BooleanObject.Value.ToString().ToLower();
        }
        else if (jsonObject is JSONCompound JSONCompound)
        {
            return SerializeCompound(JSONCompound, format, indentLevel);
        }
        else if (jsonObject is JSONArray JSONArray)
        {
            return SerializeArray(JSONArray, format, indentLevel);
        }
        else
        {
            throw new ArgumentException($"Invalid JSON object type: {jsonObject.GetType()}");
        }
    }

    private string SerializeArray(JSONArray array, bool format, int indentLevel)
    {
        if (array.Count == 0)
        {
            return $"{JSONSyntax.ARRAY_OPEN}{JSONSyntax.ARRAY_CLOSE}";
        }

        StringBuilder Data = new();
        Data.Append(JSONSyntax.ARRAY_OPEN);
        int EntryIndex = 0;
        foreach (JSONObject? Entry in array)
        {
            if (EntryIndex != 0)
            {
                Data.Append(JSONSyntax.SEPARATOR);
            }

            AppendIfFormatted(Data, '\n', format);
            AppendIndent(Data, indentLevel, format);

            Data.Append(SerializeObject(Entry, format, indentLevel + 1));
            EntryIndex++;
        }

        AppendIfFormatted(Data, '\n', format);
        AppendIndent(Data, indentLevel - 1, format);

        Data.Append(JSONSyntax.ARRAY_CLOSE);
        return Data.ToString();
    }

    private string SerializeCompound(JSONCompound compound, bool format, int indentLevel)
    {
        if (compound.EntryCount == 0)
        {
            return $"{JSONSyntax.COMPOUND_OPEN}{JSONSyntax.COMPOUND_CLOSE}";
        }

        StringBuilder Data = new();
        Data.Append(JSONSyntax.COMPOUND_OPEN);
        int EntryIndex = 0;
        foreach (KeyValuePair<string, JSONObject?> Entry in compound)
        {
            if (EntryIndex != 0)
            {
                Data.Append(JSONSyntax.SEPARATOR);
            }

            AppendIfFormatted(Data, '\n', format);
            AppendIndent(Data, indentLevel, format);

            Data.Append($"{JSONSyntax.QUOTE}{FormatString(Entry.Key)}{JSONSyntax.QUOTE}");
            Data.Append(JSONSyntax.VALUE_DEFINITION);
            AppendIfFormatted(Data, ' ', format);
            Data.Append(SerializeObject(Entry.Value, format, indentLevel + 1));
            EntryIndex++;
        }

        AppendIfFormatted(Data, '\n', format);
        AppendIndent(Data, indentLevel - 1, format);
        Data.Append(JSONSyntax.COMPOUND_CLOSE);

        return Data.ToString();
    }

    private void AppendIfFormatted(StringBuilder builder, char character, bool format)
    {
        if (format)
        {
            builder.Append(character);
        }
    }

    private void AppendIndent(StringBuilder builder, int level, bool format)
    {
        if (!format)
        {
            return;
        }

        for (int i = 0; i < level + 1; i++)
        {
            builder.Append("    ");
        }
    }

    private string FormatString(string stringToFormat)
    {
        StringBuilder FormattedData = new();

        for (int i = 0; i < stringToFormat.Length; i++)
        {
            char Character = stringToFormat[i];
            switch (Character)
            {
                case '\\':
                    FormattedData.Append("\\\\");
                    break;

                case '"':
                    FormattedData.Append("\\\"");
                    break;

                case '\n':
                    FormattedData.Append("\\n");
                    break;

                case '\r':
                    FormattedData.Append("\\r");
                    break;

                case '\t':
                    FormattedData.Append("\\t");
                    break;

                case '\f':
                    FormattedData.Append("\\f");
                    break;

                case '\b':
                    FormattedData.Append("\\b");
                    break;

                default:
                    FormattedData.Append(Character);
                    break;
            }
        }

        return FormattedData.ToString();
    }
}