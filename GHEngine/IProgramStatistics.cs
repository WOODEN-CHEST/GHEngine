﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public interface IProgramStatistics
{
    double FPS { get; }
    ulong RamUsageBytes { get; }
}