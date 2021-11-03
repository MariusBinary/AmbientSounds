using NAudio.Wave;

namespace AmbientSounds.Core
{
    public class WavePlayer
    {
        WaveFileReader Reader;
        public WaveChannel32 Channel { get; set; }

        string FileName { get; set; }

        public WavePlayer(string FileName)
        {
            this.FileName = FileName;
            Reader = new WaveFileReader(FileName);
            var loop = new LoopStream(Reader);
            Channel = new WaveChannel32(loop) { PadWithZeroes = false, Volume = 0.0f };
        }

        public void Dispose()
        {
            if (Channel != null)
            {
                Channel.Dispose();
                Reader.Dispose();
            }
        }

    }
}
