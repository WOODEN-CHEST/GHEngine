using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONBoolean : JSONObject
{
    // Fields.
    public bool Value { get; set; }


    // Constructors.
    public JSONBoolean(bool value)
    {
        Value = value;
    }


    // Operators.
    public static implicit operator bool(JSONBoolean value) => value.Value;
}
