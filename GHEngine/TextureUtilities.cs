using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public static class TextureUtilities
{
    // Static methods.
    public static Texture2D ConvertTexture(Image<Rgba32> image, GraphicsDevice graphicsDevice)
    {
        Texture2D Texture = new Texture2D(graphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color);
        Texture.SetData<Microsoft.Xna.Framework.Color>(ConvertImageToPixelArray(image));
        return Texture;
    }


    // Private static methods.
    private static Microsoft.Xna.Framework.Color[] ConvertImageToPixelArray(Image<Rgba32> image)
    {
        Microsoft.Xna.Framework.Color[] Pixels = new Microsoft.Xna.Framework.Color[image.Width * image.Height];

        int PixelIndex = 0;
        foreach (Memory<Rgba32> Memory in image.GetPixelMemoryGroup())
        {
            Span<Rgba32> PixelSpan = Memory.Span;
            for (int i = 0; i < PixelSpan.Length; i++, PixelIndex++)
            {
                Rgba32 Pixel = PixelSpan[i];
                Pixels[PixelIndex] = new Microsoft.Xna.Framework.Color(Pixel.R, Pixel.G, Pixel.B, Pixel.A);
            }
        }
        return Pixels;
    }
}