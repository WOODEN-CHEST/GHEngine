using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GHEngine.IO.DataFile;

public class GHDFCompound : IEnumerable<KeyValuePair<int, object>>
{
    // Fields.
    public int Count => _entries.Count;


    // Private fields.
    private readonly Dictionary<int, object> _entries = new();


    // Methods.
    public GHDFCompound Add(int id, object entry)
    {
        VerifyID(id);

        if (_entries.ContainsKey(id))
        {
            throw new ArgumentException($"An entry with the id {id} already exists!", nameof(id));
        }
        _entries.Add(id, entry);

        return this;
    }

    public GHDFCompound Set(int id, object entry)
    {
        VerifyID(id);
        _entries[id] = entry;

        return this;
    }

    public T? Get<T>(int id)
    {
        _entries.TryGetValue(id, out var Value);
        return ReturnCasted<T>(Value!);
    }

    public T GetVerified<T>(int id)
    {
        _entries.TryGetValue(id, out object? Value);
        if (Value == null)
        {
            throw new DataFileEntryException($"Mandatory value with ID ${id} not found.");
        }
        return ReturnCasted<T>(Value);
    }

    public T? GetOptionalVerified<T>(int id)
    {
        _entries.TryGetValue(id, out object? Value);
        if (Value == null)
        {
            return default;
        }
        return ReturnCasted<T>(Value);
    }

    public T GetOrDefault<T>(int id, T defaultValue)
    {
        if (_entries.ContainsKey(id))
        {
            return Get<T>(id)!;
        }

        return defaultValue;
    }

    public void Remove(int id) => _entries.Remove(id);

    public void Clear() => _entries.Clear();


    // Private methods.
    private void VerifyID(int id)
    {
        if (id == 0)
        {
            throw new ArgumentException("ID cannot be zero.", nameof(id));
        }
    }

    private T ReturnCasted<T>(object value)
    {
        try
        {
            return (T)value;
        }
        catch (InvalidCastException)
        {
            throw new DataFileEntryException($"Data type mismatch, value is of type \"{value.GetType()}\"," +
                $" expected \"{typeof(T)}\"");
        }
    }


    // Inherited methods.
    public IEnumerator<KeyValuePair<int, object>> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}