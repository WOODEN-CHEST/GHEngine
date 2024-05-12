using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

internal struct ZBufferedItem<T>
{
    // Felds.
    public T Item { get; init; }
    public float Index { get; init; }


    // Cónstructors.
    public ZBufferedItem(T item, float index)
    {
        Item = item;
        Index = index;
    }
}