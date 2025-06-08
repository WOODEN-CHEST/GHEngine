using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Collections;

public class ZIndexedCollection<T> : IEnumerable<T>
{
    // Fields.
    public int Count => _items.Count;


    // Private fields.
    private readonly List<ZBufferedItem<T>> _items = new();


    // Constructors.
    public ZIndexedCollection() { }


    // Methods.
    public T? GetItem(float zIndex)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Index == zIndex)
            {
                return _items[i].Item;
            }
        }

        return default;
    }

    public void RemoveItem(float zIndex)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Index == zIndex)
            {
                _items.RemoveAt(i);
            }
        }
    }

    public void RemoveItem(T item)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Item!.Equals(item))
            {
                _items.RemoveAt(i);
            }
        }
    }

    public void AddItem(T item, float zIndex)
    {
        ZBufferedItem<T> NewItem = new ZBufferedItem<T>(item, zIndex);

        if (_items.Count == 0)
        {
            _items.Add(NewItem);
            return;
        }
        else if (_items[0].Index > zIndex)
        {
            _items.Insert(0, NewItem);
            return;
        }
        else if (_items[^1].Index < zIndex)
        {
            _items.Add(NewItem);
            return;
        }

        _items.Insert(FindIndexWithNearestLowerZIndex(zIndex), NewItem);
    }

    public void ClearItems()
    {
        _items.Clear();
    }


    // Private methods.
    private int FindIndexWithNearestLowerZIndex(float zIndex)
    {
        int IndexMax = _items.Count - 1;
        int IndexMin = 0;

        while (IndexMin <= IndexMax)
        {
            int IndexMiddle = IndexMin + ((IndexMax - IndexMin) / 2);

            float ZIndexInMiddle = _items[IndexMiddle].Index;
            if (ZIndexInMiddle == zIndex)
            {
                return IndexMiddle;
            }
            else if (ZIndexInMiddle > zIndex)
            {
                IndexMax = IndexMiddle - 1;
            }
            else
            {
                IndexMin = IndexMiddle + 1;
            }
        }

        return IndexMin;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (ZBufferedItem<T> Item in _items)
        {
            yield return Item.Item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Types.
    private struct ZBufferedItem<V>
    {
        // Fields.
        public V Item { get; init; }
        public float Index { get; init; }


        // Constructors.
        public ZBufferedItem(V item, float index)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Index = index;
        }


        // Methods.
        public override string ToString()
        {
            return $"Item \"{Item?.ToString() ?? "null"}\" (${Index})";
        }
    }
}