using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONRealNumber : JSONObject
{
    // Fields.
    public double Value { get; set; }


    // Constructors.
    public JSONRealNumber(double value)
    {
        Value = value;
    }


    // Operators.
    public static implicit operator double(JSONRealNumber number) => number.Value;

    public static implicit operator float(JSONRealNumber number) => (float)number.Value;

    public static implicit operator long(JSONRealNumber value) => (long)value.Value;

    public static implicit operator ulong(JSONRealNumber value) => (ulong)value.Value;

    public static implicit operator int(JSONRealNumber value) => (int)value.Value;

    public static implicit operator uint(JSONRealNumber value) => (uint)value.Value;

    public static implicit operator short(JSONRealNumber value) => (short)value.Value;

    public static implicit operator ushort(JSONRealNumber value) => (ushort)value.Value;

    public static implicit operator byte(JSONRealNumber value) => (byte)value.Value;

    public static implicit operator sbyte(JSONRealNumber value) => (sbyte)value.Value;
}
