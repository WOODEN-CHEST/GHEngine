using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public record struct PreSampledSoundResult(double NewIndex, PreSampleSoundFinishType FinishType);