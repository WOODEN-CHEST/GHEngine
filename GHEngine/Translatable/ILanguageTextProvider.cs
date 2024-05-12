using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Translatable;

public interface ILanguageTextProvider
{
    string GetText(string key);
}