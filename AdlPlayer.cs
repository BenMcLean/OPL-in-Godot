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
                        PlayNote(Adl.Notes[CurrentNote]);
                    else
                        Adl = null;
                }
            }
        }

        public AdlPlayer ResetOctave()
        {
            OctavePortSetting = Adl.KeyFlag;
            return this;
        }

        public AdlPlayer NoteOn()
        {
            OctavePortSetting = (byte)(Adl.Block | Adl.KeyFlag);
            return this;
        }

        public AdlPlayer NoteOff()
        {
            OctavePortSetting = Adl == null ? Adl.KeyFlag : Adl.Block;
            return this;
        }

        public AdlPlayer Setup()
        {
            return SetOctave().SetInstrument().NoteOn();
        }

        public AdlPlayer SetOctave()
        {
            OctavePortSetting = (byte)(Adl.Block | OctavePortSetting);
            return this;
        }

        public byte OctavePortSetting {
            get
            {
                return octavePortSetting;
            }
            set
            {
                octavePortSetting = value;
                Opl.WriteReg(Adl.OctavePort, value);
            }
        }
        private byte octavePortSetting = 0;

        public byte NotePortSetting
        {
            get
            {
                return notePortSetting;
            }
            set
            {
                notePortSetting = value;
                Opl.WriteReg(Adl.NotePort, value);
            }
        }
        private byte notePortSetting = 0;

        public AdlPlayer SetInstrument()
        {
            for (uint i = 0; i < Adl.InstrumentPorts.Length; i++)
                Opl.WriteReg(Adl.InstrumentPorts[i], Adl.Instrument[i]);
            return this;
        }

        public AdlPlayer PlayNote(byte note)
        {
            if (note == 0)
                NoteOff();
            else if (NotePortSetting != note)
            {
                NotePortSetting = note;
                if (!IsBitSet(OctavePortSetting, Adl.KeyFlag))
                    NoteOn();
            }
            return this;
        }

        public static bool IsBitSet(byte bite, byte pos)
        {
            return (bite & (1 << pos)) != 0;
        }
    }
}
