using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Modifier;

public class PanSoundModifier : ISoundModifier
{
    // Static fields.
    public const float PAN_LEFT = -1f;
    public const float PAN_MIDDLE = 0f;
    public const float PAN_RIGHT = 1f;


    // Fields.
    public 


    // Inherited methods.
    public void Modify(float[] buffer, int count, WaveFormat targetFormat)
    {
        if (Sound.Format.Channels != 2)
        {
            throw new NotSupportedException("Panning not supported for non 2 channel sounds.");
        }

        const float MAX_VOLUME_BOOST = 0.3f; // Account for perceived loudness, however, it is still basically a random value.

        float PanLeftStrength = Math.Abs(Math.Min(pan, 0));
        float PanRightStrength = Math.Max(pan, 0);

        float RightVolumeInLeft = PanLeftStrength + PanLeftStrength * MAX_VOLUME_BOOST;
        float LeftVolumeInRight = PanRightStrength + PanRightStrength * MAX_VOLUME_BOOST;
        float LeftVolumeInLeft = 1f + PanLeftStrength * MAX_VOLUME_BOOST - PanRightStrength;
        float RightVolumeInRight = 1f + PanRightStrength * MAX_VOLUME_BOOST - PanLeftStrength;

        for (int i = 0; i < count; i += 2)
        {
            float LeftSample = buffer[i];
            float RightSample = buffer[i + 1];
            buffer[i] = LeftSample * LeftVolumeInLeft + RightSample * RightVolumeInLeft;
            buffer[i + 1] = RightSample * RightVolumeInRight + LeftSample * LeftVolumeInRight;
        }
    }
}