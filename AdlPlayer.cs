using Godot;
using NScumm.Core.Audio.OPL;
using OPL;
using OPLinGodot;

namespace OPLinGodot
{
    public static class ByteExtension
    {
        public static byte SetBit(this byte @byte, byte position, bool @bool = true)
        {
            return @bool ?
                //left-shift 1, then bitwise OR
                (byte)(@byte | (1 << position))
                //left-shift 1, then take complement, then bitwise AND
                : (byte)(@byte & ~(1 << position));
        }

        public static bool IsBitSet(this byte @byte, byte position)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return (@byte & (1 << position)) != 0;
        }
    }
}

/// <summary>
/// Plays Adlib sound effects in Godot.
/// This class does not "own" the emulated sound card. It is only responsible for adding sound effects, not collecting output.
/// </summary>
public class AdlPlayer : Node
{
    public IOpl Opl;
    public uint CurrentNote = 0;
    private float SinceLastNote = 0f;

    public Adl Adl
    {
        get
        {
            return adl;
        }
        set
        {
            adl = value;
            SinceLastNote = 0f;
            CurrentNote = 0;
            if (adl != null)
                SetInstrument().PlayNote();
        }
    }
    private Adl adl;

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
        OctavePort = (byte)(Adl.Block | Adl.KeyOnFlag);
        return this;
    }


    public AdlPlayer NoteOff()
    {
        //OctavePort = OctavePort.SetBit(Adl.KeyOnFlag, false);
        OctavePort = Adl.Block;
        return this;
    }

    public AdlPlayer Setup()
    {
        return SetInstrument().PlayNote();
    }

    public AdlPlayer SetInstrument()
    {
        for (uint i = 0; i < Adl.InstrumentPorts.Length; i++)
            Opl.WriteReg(Adl.InstrumentPorts[i], Adl.Instrument[i]);
        return this;
    }

    public AdlPlayer PlayNote()
    {
        if (Adl.Notes[CurrentNote] == 0)
            NoteOff();
        else
        {
            NoteOn();
            Opl.WriteReg(Adl.NotePort, Adl.Notes[CurrentNote]);
        }
        CurrentNote++;
        if (CurrentNote >= Adl.Notes.Length)
            Adl = null;
        return this;
    }
}
