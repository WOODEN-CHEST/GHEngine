﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public interface IColorMaskable
{
    public float Brightness { get; set; }
    public float Opacity { get; set; }
    public Color Mask { get; set; }
}