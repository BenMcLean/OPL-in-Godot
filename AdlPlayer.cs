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
        public IOpl Opl;

        public Adl Adl
        {
            get
            {
                return adl;
            }
            set
            {
                SinceLastNote = 0f;
                CurrentNote = 0;
                if (Opl != null)
                {
                    if ((adl = value) != null)
                        Setup();
                    else
                        NoteOff();
                }
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
                    PlayNote();
                }
            }
        }

        public AdlPlayer NoteOn()
        {
            Opl.WriteReg(Adl.OctavePort, (byte)(Adl.Block | Adl.KeyFlag));
            return this;
        }

        public AdlPlayer NoteOff()
        {
            Opl.WriteReg(Adl.OctavePort, 0);
            return this;
        }

        public AdlPlayer Setup()
        {
            return SetInstrument().NoteOn().PlayNote();
        }

        public AdlPlayer SetInstrument()
        {
            for (uint i = 0; i < Adl.InstrumentPorts.Length; i++)
                Opl.WriteReg(Adl.InstrumentPorts[i], Adl.Instrument[i]);
            return this;
        }

        public AdlPlayer PlayNote()
        {
            if (CurrentNote == 0 || Adl.Notes[CurrentNote] != Adl.Notes[CurrentNote - 1])
            {
                if (Adl.Notes[CurrentNote] == 0)
                    NoteOff();
                else
                {
                    Opl.WriteReg(Adl.NotePort, Adl.Notes[CurrentNote]);
                    NoteOn();
                }
            }
            CurrentNote++;
            if (CurrentNote >= Adl.Notes.Length)
                Adl = null;
            return this;
        }

        public static bool IsBitSet(byte bite, byte pos)
        {
            return (bite & (1 << pos)) != 0;
        }
    }
}
