using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

internal class GenericProgramTime : IProgramTime
{
    public TimeSpan PassedTime { get; set; }

    public TimeSpan TotalTime { get; set; }
}