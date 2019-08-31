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

        public static ushort ReadWord(this FileStream file)
        {
            return (ushort)(file.ReadByte() + (file.ReadByte() << 8));
        }

        public static ImfPacket ReadImfPacket(this FileStream file)
        {
            return new ImfPacket()
            {
                Register = (byte)file.ReadByte(),
                Data = (byte)file.ReadByte(),
                Delay = file.ReadWord()
            };
        }

        /// <summary>
        /// Reading in IMF files based on http://www.shikadi.net/moddingwiki/IMF_Format
        /// </summary>
        public static ImfPacket[] ReadImf(string filename)
        {
            ImfPacket[] imf;
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                ushort length = (ushort)(file.ReadWord() / 4); // Length is provided in number of bytes. Divide by 4 to get the number of 4 byte packets.
                if (length == 0)
                { // Type-0 format
                    file.Seek(0, 0);
                    List<ImfPacket> list = new List<ImfPacket>();
                    while (file.Position < file.Length)
                        list.Add(file.ReadImfPacket());
                    imf = list.ToArray();
                }
                else
                { // Type-1 format
                    imf = new ImfPacket[length];
                    for (uint i = 0; i < imf.Length; i++)
                        imf[i] = file.ReadImfPacket();
                }
            }
            return imf;
        }
    }
}
