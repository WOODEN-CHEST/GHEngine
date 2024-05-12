using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

internal interface IProgramStatistics
{
    float FPS { get; }
    long RamUsageBytes { get; }
}