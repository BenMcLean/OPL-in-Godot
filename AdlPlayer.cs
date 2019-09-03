using Godot;
using NScumm.Core.Audio.OPL;
using OPL;

namespace OPLinGodot
{
    /// <summary>
    /// Plays Adlib sound effects in Godot.
    /// This class does not "own" the emulated sound card. It is only responsible for adding sound effects, not collecting output.
    /// </summary>
    public class AdlPlayer : Node
    {
        public IOpl Opl { get; set; }

        public Adl Adl
        {
            get
            {
                return adl;
            }
            set
            {
                if ((adl = value) != null && Opl != null)
                    Setup();
                else
                    NoteOff();
                SinceLastNote = 0f;
                CurrentNote = 0;
            }
        }
        private Adl adl;

        public uint CurrentNote = 0;
        private float SinceLastNote = 0f;

        public override void _Process(float delta)
        {
            if (Opl != null && Adl != null)
            {
                SinceLastNote += delta;
                while (Adl != null && SinceLastNote >= Adl.Hz)
                {
                    SinceLastNote -= Adl.Hz;
                    if (++CurrentNote < Adl.Notes.Length)
                        PlayNote();
                    else
                        Adl = null;
                }
            }
        }

        public AdlPlayer ResetOctave()
        {
            OctavePort = Adl.KeyFlag;
            return this;
        }

        public AdlPlayer NoteOn()
        {
            OctavePort = (byte)(Adl.KeyFlag | OctavePort);
            return this;
        }

        public AdlPlayer NoteOff()
        {
            OctavePort = 0;
            return this;
        }

        public AdlPlayer Setup()
        {
            return SetInstrument().NoteOn();
        }

        public AdlPlayer SetOctave()
        {
            OctavePort = (byte)(Adl.Block | OctavePort);
            return this;
        }

        public byte OctavePort
        {
            get
            {
                return octavePort;
            }
            set
            {
                octavePort = value;
                Opl.WriteReg(Adl.OctavePort, value);
            }
        }
        private byte octavePort = 0;

        public byte NotePort
        {
            get
            {
                return notePort;
            }
            set
            {
                notePort = value;
                Opl.WriteReg(Adl.NotePort, value);
            }
        }
        private byte notePort = 0;

        public AdlPlayer SetInstrument()
        {
            for (uint i = 0; i < Adl.InstrumentPorts.Length; i++)
                Opl.WriteReg(Adl.InstrumentPorts[i], Adl.Instrument[i]);
            return this;
        }

        public AdlPlayer PlayNote()
        {
            if (CurrentNote > 0 && Adl.Notes[CurrentNote] == Adl.Notes[CurrentNote - 1])
                return this;
            if (Adl.Notes[CurrentNote] == 0)
                NoteOff();
            else
            {
                NotePort = Adl.Notes[CurrentNote];
                NoteOn();
                SetOctave();
            }
            return this;
        }

        public static bool IsBitSet(byte bite, byte pos)
        {
            return (bite & (1 << pos)) != 0;
        }
    }
}
