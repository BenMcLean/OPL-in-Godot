using Godot;
using NScumm.Core.Audio.OPL;
using static OPL.Imf;

namespace OPLinGodot
{
    /// <summary>
    /// Plays back IMF songs in Godot.
    /// This class is assumed to "own" the emulated sound card since it is responsible both for music data input and for sound output.
    /// </summary>
    public class ImfPlayer : Node
    {
        public IOpl Opl { get; set; }

        public bool Music { get; set; } = true;
        public bool Loop { get; set; } = true;

        private AudioStreamPlayer audioStreamPlayer;
        public AudioStreamPlayer AudioStreamPlayer
        {
            get
            {
                return audioStreamPlayer;
            }
            set
            {
                audioStreamPlayer = value;
                value.Stream = new AudioStreamGenerator()
                {
                    MixRate = Main.MixRate
                };
            }
        }

        public AudioStreamGeneratorPlayback AudioStreamGeneratorPlayback
        {
            get
            {
                return (AudioStreamGeneratorPlayback)AudioStreamPlayer?.GetStreamPlayback();
            }
        }

        public AudioStreamGenerator AudioStreamGenerator
        {
            get
            {
                return (AudioStreamGenerator)AudioStreamPlayer?.GetStream();
            }
        }

        public override void _Ready()
        {
            base._Ready();
            AudioStreamPlayer.Play();
            if (Music)
                PlayNotes(float.Epsilon);
            FillBuffer();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            // Input
            if (AudioStreamPlayer.Playing && Music)
                PlayNotes(delta);
            // Output
            FillBuffer();
        }

        public ImfPlayer PlayNotes(float delta)
        {
            if (AudioStreamPlayer.Playing)
            {
                TimeSinceLastPacket += delta;
                if (TimeSinceLastPacket >= CurrentPacketDelay)
                {
                    TimeSinceLastPacket -= CurrentPacketDelay;
                    do
                    {
                        if (++CurrentPacket < Song.Length)
                            Opl.WriteReg(Song[CurrentPacket].Register, Song[CurrentPacket].Data);
                    }
                    while (CurrentPacket < Song.Length && Song[CurrentPacket].Delay == 0);
                    CurrentPacketDelay = CurrentPacket < Song.Length ?
                        Delay(Song[CurrentPacket].Delay)
                        : 0;
                }
                if (Loop && CurrentPacket >= Song.Length)
                    Song = Song;
            }
            return this;
        }

        public ImfPlayer FillBuffer()
        {
            int toFill = AudioStreamGeneratorPlayback.GetFramesAvailable();
            if (Opl.IsStereo)
            {
                if (Buffer.Length < toFill * 2)
                    Buffer = new short[toFill * 2];
                Opl.ReadBuffer(Buffer, 0, toFill * 2);
            }
            else
            {
                if (Buffer.Length < toFill)
                    Buffer = new short[toFill];
                Opl.ReadBuffer(Buffer, 0, toFill);
            }
            for (uint i = 0; i < toFill; i++)
            {
                float soundbite = Buffer[i] / 32767f; // Convert from 16 bit signed integer audio to 32 bit signed float audio
                Vector2.Set(soundbite, soundbite);
                AudioStreamGeneratorPlayback.PushFrame(Vector2);
            }
            return this;
        }

        private Vector2 Vector2 = new Vector2(0f, 0f);

        private short[] Buffer = new short[70000];

        public int BufferSize
        {
            get
            {
                return Buffer.Length;
            }
        }

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
                CurrentPacketDelay = 0f;
                TimeSinceLastPacket = 0f;
            }
        }
        private ImfPacket[] song;

        private float CurrentPacketDelay = 0f;

        public uint CurrentPacket { get; set; } = 0;

        public float TimeSinceLastPacket { get; set; } = 0f;
    }
}
