using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public interface IAssetStorage : IAssetProvider
{
    // Fields.
    public IAssetDefinitionCollection Definitions { get; set; }
    public IAsset
}