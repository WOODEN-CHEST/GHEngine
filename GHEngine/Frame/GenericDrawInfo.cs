﻿using GHEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

internal class GenericDrawInfo : IDrawInfo
{
    public SpriteBatch SpriteBatch { get; set; }

    public IProgramTime Time { get; set; }
}