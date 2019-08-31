using Godot;
using NScumm.Core.Audio.OPL;
using static OPL.Imf;

namespace OPLinGodot
{
    public class ImfPlayer : Node
    {
        public AudioStreamPlayer AudioStreamPlayer { get; set; }
        public AudioStreamGenerator AudioStreamGenerator { get; set; }
        public AudioStreamGeneratorPlayback AudioStreamGeneratorPlayback
        {
            get
            {
                return AudioStreamGeneratorPlayback == null && AudioStreamPlayer != null ?
                    (AudioStreamGeneratorPlayback)AudioStreamPlayer.GetStreamPlayback()
                    : null;
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            // Input
            if (AudioStreamPlayer.Playing)
            {
                TimeSinceLastPacket += delta;
                if (TimeSinceLastPacket >= CurrentPacketDelay)
                {
                    TimeSinceLastPacket -= CurrentPacketDelay;
                    do
                    {
                        CurrentPacket++;
                        if (CurrentPacket < Song.Length)
                            Opl.WriteReg(Song[CurrentPacket].Register, Song[CurrentPacket].Data);
                    }
                    while (CurrentPacket < Song.Length && Song[CurrentPacket].Delay == 0);
                    CurrentPacketDelay = Delay(Song[CurrentPacket].Delay);
                }
            }

            // Output
            int delay = (int)(delta * 700f);
            if (AudioStreamGeneratorPlayback.CanPushBuffer(delay))
            {
                if (buffer.Length < delay)
                    buffer = new short[delay];
                Opl.ReadBuffer(buffer, 0, delay);
                if (frames.Length < delay)
                    frames = new Vector2[delay];
                for (uint i=0; i<delay; i++)
                {
                    if (frames[i] == null)
                        frames[i] = new Vector2(buffer[i], buffer[i]);
                    else
                    {
                        frames[i].x = buffer[i];
                        frames[i].y = buffer[i];
                    }
                    AudioStreamGeneratorPlayback.PushFrame(frames[i]);
                }
            }
        }

        short[] buffer = new short[700];
        Vector2[] frames = new Vector2[700];

        public IOpl Opl { get; set; }

        public ImfPacket[] Song
        {
            get
            {
                return song;
            }
            set
            {
                song = value;
                CurrentPacket = 0;
                CurrentPacketDelay = Delay(song[CurrentPacket].Delay);
                TimeSinceLastPacket = 0f;
                //SongLength = 0f;
                //foreach (ImfPacket packet in value)
                //    SongLength += Delay(packet.Delay);
            }
        }
        private ImfPacket[] song;


        public static float Delay(ushort time)
        {
            return time / 700f; // Wolf3D song notes happen at 700 hz.
        }

        private float CurrentPacketDelay = 0f;

        public uint CurrentPacket
        {
            get
            {
                return currentPacket;
            }
            set
            {
                currentPacket = value % (uint)Song.Length;
            }
        }
        private uint currentPacket = 0;

        public float TimeSinceLastPacket { get; set; } = 0f;

        //public float SongLength
        //{
        //    get
        //    {
        //        return SongLength;
        //    }
        //    private set
        //    {
        //        SongLength = value;
        //    }
        //}
    }
}
