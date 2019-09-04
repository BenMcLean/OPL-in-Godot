using NScumm.Core.Audio.OPL;
using SimpleLogger;

namespace OPL
{
    public class OplLogger : IOpl
    {
        public IOpl Opl;

        public OplLogger(IOpl Opl)
        {
            this.Opl = Opl;
        }

        public bool IsStereo
        {
            get
            {
                return Opl.IsStereo;
            }
        }

        public void Init(int rate)
        {
            Logger.Log("Init(" + rate + ");");
            Opl.Init(rate);
        }

        public void ReadBuffer(short[] buffer, int pos, int length)
        {
            Opl.ReadBuffer(buffer, pos, length);
        }

        public void WriteReg(int r, int v)
        {
            Logger.Log("WriteReg(" + r + ", " + v + ");");
            Opl.WriteReg(r, v);
        }
    }
}
