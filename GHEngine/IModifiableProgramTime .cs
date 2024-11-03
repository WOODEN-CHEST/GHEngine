using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public interface IModifiableProgramTime : IProgramTime
{
    // Fields.
    new TimeSpan PassedTime { get; set; }
    new TimeSpan TotalTime { get; set; }
}