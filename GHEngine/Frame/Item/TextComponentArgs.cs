using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame.Item;

public class TextComponentArgs : EventArgs
{
    // Fields.
    public TextComponent Component { get; private init; }


    // Constructors.
    public TextComponentArgs(TextComponent component)
    {
        Component = component ?? throw new ArgumentNullException(nameof(component));
    }
}