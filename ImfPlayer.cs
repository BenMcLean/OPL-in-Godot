﻿using Godot;
using NScumm.Core.Audio.OPL;
using static OPL.Imf;

namespace OPLinGodot
{
    public class ImfPlayer : Node
    {
        public IOpl Opl { get; set; }
        private readonly int hz = 48000;

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
                    MixRate = hz
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
            Opl.Init(hz);
            FillBuffer();
            AudioStreamPlayer.Play();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            // Input
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
                        CurrentPacket++;
                        if (CurrentPacket < Song.Length)
                            Opl.WriteReg(Song[CurrentPacket].Register, Song[CurrentPacket].Data);
                    }
                    while (CurrentPacket < Song.Length && Song[CurrentPacket].Delay == 0);
                    CurrentPacketDelay = Delay(Song[CurrentPacket].Delay);
                }
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
                float foo = Buffer[i] / 32767f;
                Vector2 vector2 = Vector2Pool.Get();
                vector2.Set(foo, foo);
                AudioStreamGeneratorPlayback.PushFrame(vector2);
                Vector2Pool.Return(vector2);
            }
            return this;
        }

        short[] Buffer = new short[512];

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
                currentPacket = value;
            }
        }
        private uint currentPacket = 0;

        public float TimeSinceLastPacket { get; set; } = 0f;
    }
}
