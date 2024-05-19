using System.Collections;
using System.Text;

namespace GHEngine.IO.JSON;


public class JSONArray : JSONObject, IList<JSONObject?>
{
    // Fields.
    public int Count => _values.Count;
    public bool IsReadOnly => false;


    // Private fields.
    private readonly List<JSONObject?> _values = new();


    // Constructors.
    public JSONArray() { }


    // Inherited methods.
    public void Add(JSONObject? value)
    {
        _values.Add(value);
    }

    public void Clear() => _values.Clear();

    public int IndexOf(JSONObject? item)
    {
        return _values.IndexOf(item);
    }

    public void Insert(int index, JSONObject? item)
    {
        _values.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _values.RemoveAt(index);
    }

    public bool Contains(JSONObject? item)
    {
        return _values.Contains(item);
    }

    public void CopyTo(JSONObject?[] array, int arrayIndex)
    {
        _values.CopyTo(array, arrayIndex);
    }

    bool ICollection<JSONObject?>.Remove(JSONObject? item)
    {
        return _values.Remove(item);
    }

    public IEnumerator<JSONObject?> GetEnumerator()
    {
        foreach (JSONObject? Value in _values)
        {
            yield return Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Operators.
    public JSONObject? this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }
}