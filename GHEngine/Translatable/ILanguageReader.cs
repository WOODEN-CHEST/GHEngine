using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Translatable;

public interface ILanguageReader
{
    public void Read(ILanguage language, string filePath);

    public void Read(ILanguage language, Stream dataStream);
}