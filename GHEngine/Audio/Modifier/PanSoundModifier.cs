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

    public const int SUPPORTED_CHANNEL_COUNT = 2;


    // Fields.
    public float Pan
    {
        get => _pan;
        set
        {
            _pan = float.IsNaN(value) ? PAN_MIDDLE : Math.Clamp(value, PAN_LEFT, PAN_RIGHT);
        }
    }


    // Private fields.
    private float _pan = PAN_MIDDLE;


    // Inherited methods.
    public bool Modify(float[] buffer, int count, WaveFormat targetFormat)
    {
        if (targetFormat.Channels != 2)
        {
            throw new AudioModifyException("Panning not supported for non 2 channel sounds.");
        }
        if (count % SUPPORTED_CHANNEL_COUNT != 0)
        {
            throw new AudioModifyException($"Invalid number of samples requested to modify: {count}");
        }

        const float MAX_VOLUME_BOOST = 0.3f; // Account for perceived loudness, however, it is still basically a random value.

        float PanLeftStrength = Math.Abs(Math.Min(Pan, 0f));
        float PanRightStrength = Math.Max(Pan, 0f);

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

        return false;
    }
}