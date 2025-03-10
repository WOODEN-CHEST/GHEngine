using System.Collections;
using System.Diagnostics.CodeAnalysis;


namespace GHEngine.Collections;

public class RandomSequence<T> : IEnumerable<T?>
{
    // Fields.
    public int Count => _items.Length;


    // Private fields.
    private T?[] _items;
    private int _index;


    // Constructors.
    public RandomSequence(T?[] items)
    {
        SetItems(items);
    }


    // Methods.
    public T? Get()
    {
        if (_items.Length == 0)
        {
            return default;
        }

        if (_index >= _items.Length)
        {
            Randomize();
        }

        return _items[_index++];
    }

    public void Randomize()
    {
        int RandIndex;

        for (int Index = 0; Index < _items.Length / 2; Index++)
        {
            RandIndex = Random.Shared.Next(_items.Length);
            (_items[RandIndex], _items[Index]) = (_items[Index], _items[RandIndex]);
        }

        _index = 0;
    }

    public void SetItems(T?[] items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        Randomize();
    }


    // Inherited methods.
    public IEnumerator<T?> GetEnumerator()
    {
        RandomSequence<T?> NewSequence = new(_items);
        for (int i = 0; i < NewSequence.Count; i++)
        {
            yield return NewSequence.Get();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Operators.
    public T? this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
}