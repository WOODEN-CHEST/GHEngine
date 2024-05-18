using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONString : JSONObject
{
    // Fields.
    public string Value
    {
        get => _value;
        set => _value = value ?? throw new ArgumentNullException(nameof(value));
    }


    // Private fields.
    private string _value;


    // Constructors.
    public JSONString(string value)
    {
        Value = value;
    }


    // Operators.
    public static implicit operator string(JSONString value) => value.Value;
}
