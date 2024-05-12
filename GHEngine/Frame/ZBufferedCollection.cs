using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public class ZBufferedCollection<T>
{
    // Private fields.
    private readonly List<ZBufferedItem<T>> _items = new();


    // Constructors.
    public ZBufferedCollection() { }


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
    private int FindIndexWithNearestLowerZIndex(float zindex)
    {
        int Index = _items.Count / 2;
        int Step = _items.Count / 2;

        while (Step != 0)
        {
            if (_items[Index].Index < zindex)
            {
                Index += Step;
            }
            else
            {
                Index -= Step;
            }

            Step /= 2;
        }

        return Index;
    }
}