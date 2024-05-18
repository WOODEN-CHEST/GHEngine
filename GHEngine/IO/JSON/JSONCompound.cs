using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONCompound : JSONObject, IEnumerable<KeyValuePair<string, JSONObject?>>
{
    // Fields.
    public int EntryCount => _entries.Count;


    // Private fields.
    private readonly Dictionary<string, JSONObject?> _entries = new();


    // Constructors.
    public JSONCompound() { }


    // Methods.
    public void AddEntry(string key, JSONObject? value)
    {
        _entries.Add(key,value);
    }

    public bool RemoveEntry(string key)
    {
        return _entries.Remove(key);
    }

    public void ClearEntries()
    {
        _entries.Clear();
    }


    public T? GetEntry<T>(string key) where T : JSONObject
    {
        _entries.TryGetValue(key, out JSONObject? Value);
        return Value as T;
    }

    public T? GetVerifiedEntry<T>(string key) where T : JSONObject
    {
        _entries.TryGetValue(key, out JSONObject? Value);

        if (Value == null)
        {
            throw new JSONEntryException($"Missing mandatory entry with key \"{key}\" of type \"{typeof(T)}\"");
        }
        return ReturnCastedObject<T>(key, Value);
    }

    public T? GetOptionalVerifiedEntry<T>(string key) where T : JSONObject
    {
        _entries.TryGetValue(key, out JSONObject? Value);
        if (Value == null)
        {
            return null;
        }
        return ReturnCastedObject<T>(key, Value);
    }


    // Private methods.
    private T ReturnCastedObject<T>(string key, JSONObject value) where T : JSONObject
    {
        if (value is T)
        {
            return (T)value;
        }
        throw new JSONEntryException($"Entry with key \"{key}\" is of type {value.GetType()}, expected {typeof(T)}.");
    }



    // Inherited methods.
    public IEnumerator<KeyValuePair<string, JSONObject?>> GetEnumerator()
    {
        foreach (KeyValuePair<string, JSONObject?> Pair in _entries)
        {
            yield return Pair;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }




    // Operators.
    public JSONObject? this[string key]
    {
        get => _entries[key];
        set => _entries[key] = value;
    }
}