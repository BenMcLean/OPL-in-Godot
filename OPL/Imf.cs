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
            public byte Register { get; set; } // Sent to register port.
            public byte Data { get; set; } // Sent to data port.
            public ushort Delay { get; set; } // How much to wait.
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

        public static ImfPacket[] ReadImf(string filename)
        {
            ImfPacket[] imf;
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                ushort length = (ushort)(file.ReadWord() / 4); // divide by 4 for the 4 byte packets
                if (length == 0)
                {
                    file.Seek(0, 0);
                    List<ImfPacket> list = new List<ImfPacket>();
                    while (file.Position < file.Length)
                        list.Add(file.ReadImfPacket());
                    imf = list.ToArray();
                }
                else
                {
                    imf = new ImfPacket[length];
                    for (uint i = 0; i < imf.Length; i++)
                        imf[i] = file.ReadImfPacket();
                }
            }
            return imf;
        }
    }
}
