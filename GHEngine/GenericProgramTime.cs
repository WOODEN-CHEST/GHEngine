using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public class GenericProgramTime : IModifiableProgramTime
{
    public TimeSpan PassedTime { get; set; }

    public TimeSpan TotalTime { get; set; }
}