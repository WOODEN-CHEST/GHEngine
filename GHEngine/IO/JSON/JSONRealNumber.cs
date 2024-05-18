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
}
