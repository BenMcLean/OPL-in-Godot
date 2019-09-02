using Godot;
using NScumm.Core.Audio.OPL;
using OPL;

namespace OPLinGodot
{
    /// <summary>
    /// Plays Adlib sound effects in Godot.
    /// This class does not "own" the emulated sound card. It is only responsible for adding sound effects, not collecting output.
    /// </summary>
    class AdlPlayer : Node
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
                    adl.Setup(Opl);
                SinceLastNote = 0f;
                CurrentNote = 0;
            }
        }
        private Adl adl;

        private uint CurrentNote = 0;
        private float SinceLastNote = 0f;

        public override void _Process(float delta)
        {
            if (Opl != null && Adl != null && CurrentNote < Adl.Notes.Length)
            {
                SinceLastNote += delta;
                while (SinceLastNote >= Adl.Hz)
                {
                    SinceLastNote -= Adl.Hz;
                    Adl.PlayNote(Opl, Adl.Notes[CurrentNote]);
                }
            }
        }
    }
}
