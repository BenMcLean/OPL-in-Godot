using NScumm.Core.Audio.OPL;
using System.IO;

namespace OPL
{
    /// <summary>
    /// Parses and stores the Adlib sound effect format. http://www.shikadi.net/moddingwiki/Adlib_sound_effect
    /// This format is extracted from Adam Biser's Wolfenstein Data Compiler (WDC) with the file extension ".ADL"
    /// However, it is not related to the ADL format by Westwood, despite doing the exact same job on the exact same hardware with the exact same file extension.
    /// </summary>
    public class Adl
    {
        /// <summary>
        /// The OPL register settings for the instrument
        /// </summary>
        public byte[] Instrument = new byte[16];

        /// <summary>
        /// Octave to play notes at
        /// </summary>
        public byte Octave;

        /// <summary>
        /// Pitch data
        /// </summary>
        public byte[] Notes;

        /// <param name="notes">How many notes to read in, or 0 to read till end of stream</param>
        public Adl(Stream stream, uint notes = 0)
        {
            stream.Read(Instrument, 0, Instrument.Length);
            Octave = (byte)stream.ReadByte();
            Notes = new byte[notes == 0 ? stream.Length - stream.Position : notes];
            stream.Read(Notes, 0, Notes.Length);
        }

        public byte Block
        {
            get
            {
                return (byte)((Octave & 7) << 2);
            }
        }

        public static readonly byte KeyFlag = 0x20;

        public Adl ResetOctave(IOpl opl)
        {
            opl.WriteReg(OctavePort, KeyFlag);
            return this;
        }

        public Adl NoteOn(IOpl opl)
        {
            opl.WriteReg(OctavePort, Block | KeyFlag);
            return this;
        }

        public Adl NoteOff(IOpl opl)
        {
            opl.WriteReg(OctavePort, Block);
            return this;
        }

        public Adl Setup(IOpl opl)
        {
            return SetOctave(opl).SetInstrument(opl);
        }

        public Adl SetOctave(IOpl opl)
        {
            opl.WriteReg(OctavePort, Block | opl.Read(OctavePort));
            return this;
        }

        public Adl SetInstrument(IOpl opl)
        {
            for (uint i = 0; i < InstrumentPorts.Length; i++)
                opl.WriteReg(InstrumentPorts[i], Instrument[i]);
            return this;
        }

        public void PlayNote(IOpl opl, byte note)
        {
            if (note == 0)
                NoteOff(opl);
            else if (opl.Read(NotePort) != note)
            {
                opl.WriteReg(NotePort, note);
                if (!IsBitSet(opl.Read(OctavePort), KeyFlag))
                    NoteOn(opl);
            }
        }

        public static bool IsBitSet(byte bite, byte pos)
        {
            return (bite & (1 << pos)) != 0;
        }

        public static readonly float Hz = 1f / 140f; // These sound effects play back at 140 Hz.

        public static readonly byte NotePort = 0xA0;
        public static readonly byte OctavePort = 0xB0;

        public static readonly byte[] InstrumentPorts = new byte[]
        {
            0x20, // mChar 	0x20 	Modulator characteristics
            0x23, // cChar 	0x23 	Carrier characteristics
            0x40, // mScale 	0x40 	Modulator scale
            0x43, // cScale 	0x43 	Carrier scale
            0x60, // mAttack 	0x60 	Modulator attack/decay rate
            0x63, // cAttack 	0x63 	Carrier attack/decay rate
            0x80, // mSust 	0x80 	Modulator sustain
            0x83, // cSust 	0x83 	Carrier sustain
            0xE0, // mWave 	0xE0 	Modulator waveform
            0xE3, // cWave 	0xE3 	Carrier waveform
            0xC0, // nConn 	0xC0 	Feedback/connection (usually ignored and set to 0)
                  // voice 	- 	unknown (Muse-only)
                  // mode 	- 	unknown (Muse-only)
                  //UINT8[3] 	padding 	- 	Pad instrument definition up to 16 bytes
        };
    }
}
