using GHEngine.Assets;
using GHEngine.Audio;
using GHEngine.Frame;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GHEngine;

internal class GHGameAsset
{
    // Fields.
    internal AssetType Type { get; private init; }
    internal object Asset { get; }


    // Private fields.
    private readonly HashSet<object> _users = new(4);


    // Constructors.
    public GhGameAsset(AssetType type, object asset)
    {
        Type = type;
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));
    }
}