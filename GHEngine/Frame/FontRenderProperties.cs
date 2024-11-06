using GHEngine.GameFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public record class FontRenderProperties(GHFontFamily FontFamily,
    bool IsBold,
    bool IsItalic,
    float LineSpacing,
    float CharSpacing);