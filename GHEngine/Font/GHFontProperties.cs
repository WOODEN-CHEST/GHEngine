using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.GameFont;

public record class GHFontProperties(float Size, bool IsBold, bool IsItalic, float LineSpacing, float CharSpacing) { }