﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public class AudioSampleException : Exception
{
    public AudioSampleException(string? message) : base(message) { }
}