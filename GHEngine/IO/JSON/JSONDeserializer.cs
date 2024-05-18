using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONDeserializer
{
    // Constructors.
    public JSONDeserializer() { }


    // Methods.
    public JSONObject? Deserialize(string data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        SerializationState State = new(data);
        SkipUnitlNowWhiteSpace(State);
        return ParseValue(State);
    }


    // Private methods.
    private void SkipUnitlNowWhiteSpace(SerializationState state)
    {
        while (!char.IsWhiteSpace(state.GetChar()) && state.IsIndexInBounds)
        {
            state.IncrementIndex();
        }
    }

    private JSONObject? ParseValue(SerializationState state)
    {
        char Character = state.GetChar();
        if (Character == JSONSyntax.COMPOUND_OPEN)
        {
            return ParseCompound(state);
        }
        else if (Character == JSONSyntax.ARRAY_OPEN)
        {
            return ParseArray(state);
        }
        else if (char.IsAsciiLetter(Character))
        {
            return ParseLiteralValue(state);
        }
        else if (char.IsDigit(Character) || (Character == JSONSyntax.NUMBER_SIGN_PLUS)
            || (Character == JSONSyntax.NUMBER_SIGN_MINUS))
        {
            return ParseNumber(state);
        }
        else
        {
            throw new JSONDeserializeException("Invalid value", state.Line, state.Column);
        }
    }

    private JSONCompound ParseCompound(SerializationState state)
    {
        if (state.GetChar() != JSONSyntax.COMPOUND_OPEN)
        {
            throw new JSONDeserializeException($"Expected '{JSONSyntax.COMPOUND_OPEN}'.", state.Line, state.Column);
        }
        state.IncrementIndex();

        SkipUnitlNowWhiteSpace(state);
        while ((state.GetChar() != JSONSyntax.COMPOUND_OPEN) && state.IsIndexInBounds)
        {
            if ()



        }


        if (state.GetChar() != JSONSyntax.COMPOUND_CLOSE)
        {
            throw new JSONDeserializeException($"Expected '{JSONSyntax.COMPOUND_CLOSE}'.", state.Line, state.Column);
        }
    }

    private JSONArray ParseArray(SerializationState state)
    {

    }

    private string ParseString(SerializationState state)
    {
        if (state.GetChar() != '"')
        {
            throw new JSONDeserializeException("Missing starting quote for string.", state.Line, state.Column);
        }

        StringBuilder Builder = new();
        bool IsInEscapeSequence = false;
        while (((state.GetChar() != JSONSyntax.QUOTE) || IsInEscapeSequence) && state.IsIndexInBounds)
        {
            char Character = state.GetChar();

            IsInEscapeSequence = !IsInEscapeSequence && (Character == JSONSyntax.ESCAPE_CHARACTER);
            


            state.IncrementIndex();
        }

        if (state.GetChar() != '"')
        {
            throw new JSONDeserializeException("Missing ending quote for string.", state.Line, state.Column);
        }
        return Builder.ToString();
    }

    private JSONObject? ParseLiteralValue(SerializationState state)
    {
        StringBuilder Builder = new();
        while (char.IsAsciiLetter(state.GetChar()))
        {
            Builder.Append(state.GetChar());
            state.IncrementIndex();
        }
        string Word = Builder.ToString();

        switch (Word)
        {
            case "null":
                return null;

            case "true":
                return new JSONBoolean(true);

            case "false":
                return new JSONBoolean(false);

            default:
                throw new JSONDeserializeException($"Unknown literal \"{Word}\".", state.Line, state.Column);
        }
    }

    private JSONObject ParseNumber(SerializationState state)
    {
        StringBuilder Builder = new();
        while (char.IsDigit(state.GetChar()) || (state.GetChar() == '.') || (char.ToLower(state.GetChar()) == 'e')
            || (state.GetChar() == '+') || (state.GetChar() == '-'))
        {
            Builder.Append(state.GetChar());
            state.IncrementIndex();
        }
        string Number = Builder.ToString();

        if (long.TryParse(Number, CultureInfo.InvariantCulture, out long LongValue))
        {
            return new JSONInteger(LongValue);
        }
        else if (double.TryParse(Number, CultureInfo.InvariantCulture, out double DoubleValue))
        {
            return new JSONRealNumber(DoubleValue);
        }
        else
        {
            throw new JSONDeserializeException($"Couldn't parse number \"{Number}\"", state.Line, state.Column);
        }
    }


    // Types.
    private class SerializationState
    {
        // Fields.
        public string Data { get; }
        public int Index { get; private set; } = 0;
        public int Line { get; private set; } = 1;
        public int Column { get; private set; } = 1;
        public bool IsIndexInBounds => Index < Data.Length;


        // Constructors.
        public SerializationState(string data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }


        // Methods.
        public char GetChar()
        {
            return Index >= Data.Length ? '\0' : Data[Index];
        }

        public void IncrementIndex()
        {
            if (GetChar() == '\n')
            {
                Line++;
                Column = 1;
            }
            else
            {
                Column++;
            }
            Index++;
        }
    }
}