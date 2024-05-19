﻿using GHEngine.Frame.Item;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public class GHLayer : ILayer
{
    // Fields.
    public string Name { get; private init; }
    public bool IsVisible { get; set; }
    public SpriteEffect? Shader { get; set; }

    public float Brightness
    {
        get => _colorMask.Brightness;
        set => _colorMask.Brightness = value;
    }

    public float Opacity
    {
        get => _colorMask.Opacity;
        set => _colorMask.Opacity = value;
    }

    public Color Mask
    {
        get => _colorMask.Mask;
        set => _colorMask.Mask = value;
    }

    public IRenderableItem[] Items => _renderableItems.ToArray();
    public int DrawableItemCount => _renderableItems.Count;


    // Private fields.
    private readonly ZIndexedCollection<IRenderableItem> _renderableItems = new();
    private GenericColorMask _colorMask = new();


    // Constructors.
    public GHLayer(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }


    // Inherited  methods.
    public void AddItem(IRenderableItem item)
    {
        AddItem(item, ILayer.DEFAULT_Z_INDEX);
    }

    public void AddItem(IRenderableItem item, float zIndex)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _renderableItems.AddItem(item, zIndex);
    }

    public void RemoveItem(IRenderableItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _renderableItems.RemoveItem(item);
    }

    public void ClearItems()
    {
        _renderableItems.ClearItems();
    }

    public void Render(IRenderer rendered, IProgramTime time)
    {
        foreach (IRenderableItem Item in _renderableItems)
        {
            Item.Render(rendered, time);
        }
    }
}