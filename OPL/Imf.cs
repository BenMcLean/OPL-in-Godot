using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPL
{
    public static class Imf
    {
        public struct ImfPacket
        {
            /// <summary>
            /// Sent to register port.
            /// </summary>
            public byte Register { get; set; }

            /// <summary>
            /// Sent to data port.
            /// </summary>
            public byte Data { get; set; }

            /// <summary>
            /// How much to wait.
            /// </summary>
            public ushort Delay { get; set; }
        }

        public static ushort ReadWord(this Stream stream)
        {
            return (ushort)(stream.ReadByte() + (stream.ReadByte() << 8));
        }

        public static ImfPacket ReadImfPacket(this Stream stream)
        {
            return new ImfPacket()
            {
                Register = (byte)stream.ReadByte(),
                Data = (byte)stream.ReadByte(),
                Delay = stream.ReadWord()
            };
        }

        /// <summary>
        /// Reading in IMF files based on http://www.shikadi.net/moddingwiki/IMF_Format
        /// </summary>
        public static ImfPacket[] ReadImf(Stream stream)
        {
            ImfPacket[] imf;
            ushort length = (ushort)(stream.ReadWord() / 4); // Length is provided in number of bytes. Divide by 4 to get the number of 4 byte packets.
            if (length == 0)
            { // Type-0 format
                stream.Seek(0, 0);
                List<ImfPacket> list = new List<ImfPacket>();
                while (stream.Position < stream.Length)
                    list.Add(stream.ReadImfPacket());
                imf = list.ToArray();
            }
            else
            { // Type-1 format
                imf = new ImfPacket[length];
                for (uint i = 0; i < imf.Length; i++)
                    imf[i] = stream.ReadImfPacket();
            }
            return imf;
        }
    }
}
