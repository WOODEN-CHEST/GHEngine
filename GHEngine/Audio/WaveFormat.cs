namespace GHEngine.Audio
{
    public class WaveFormat
    {
        // Fields.
        public int SampleRate { get; private init; }
        public int Channels { get; private init; }
        public int BitsPerSample { get; private init; }


        // Constructors.
        public WaveFormat(int sampleRate, int channels, int bitsPerSample)
        {
            SampleRate = sampleRate;
            Channels = channels;
            BitsPerSample = bitsPerSample;
        }
    }
}