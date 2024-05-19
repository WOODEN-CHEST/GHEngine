using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONInteger : JSONObject
{
    // Fields.
    public long Value { get; set; }


    // Constructors.
    public JSONInteger(long value)
    {
        Value = value;
    }


    // Operators.
    public static implicit operator float(JSONInteger value) => value.Value;

    public static implicit operator double(JSONInteger value) => value.Value;

    public static implicit operator long (JSONInteger value) => value.Value;

    public static implicit operator ulong (JSONInteger value) => (ulong)value.Value;

    public static implicit operator int (JSONInteger value) => (int)value.Value;

    public static implicit operator uint (JSONInteger value) => (uint)value.Value;

    public static implicit operator short (JSONInteger value) => (short)value.Value;

    public static implicit operator ushort (JSONInteger value) => (ushort)value.Value;

    public static implicit operator byte (JSONInteger value) => (byte)value.Value;

    public static implicit operator sbyte (JSONInteger value) => (sbyte)value.Value;
}