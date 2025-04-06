using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public interface IAssetDefinitionConverter
{
    void Read(IAssetDefinitionCollection definitions, string directoryPath);
    void Read(IAssetDefinitionCollection definitions, Stream dataStream);
    void Write(IAssetDefinitionCollection definitions, string assetFilePath);
    void Write(IAssetDefinitionCollection definition, Stream dataStream);
}