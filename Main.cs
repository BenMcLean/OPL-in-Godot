using Godot;
using NScumm.Core.Audio.OPL;
using NScumm.Core.Audio.OPL.DosBox;
using OPL;
using OPLinGodot;
using System.IO;
using static OPL.Imf;

public class Main : Control
{
    ImfPlayer ImfPlayer;

    public override void _Ready()
    {
        using (FileStream file = new FileStream("WONDERIN_MUS.imf", FileMode.Open))
            ImfPlayer = new ImfPlayer()
            {
                Opl = new DosBoxOPL(OplType.Opl3),
                Song = ReadImf(file),
                AudioStreamPlayer = new AudioStreamPlayer()
            };
        AddChild(ImfPlayer);
        AddChild(ImfPlayer.AudioStreamPlayer);

        Button button = new PlayButton
        {
            Text = "Stop",
            AudioStreamPlayer = ImfPlayer.AudioStreamPlayer
        };
        AddChild(button);

        SongStep songStep = new SongStep
        {
            ImfPlayer = ImfPlayer
        };
        AddChild(songStep);


        using (FileStream file = new FileStream("GETAMMOSND.adl", FileMode.Open))
            Adl = new Adl(file);
    }

    public Adl Adl;

    //public override void _Process(float delta)
    //{

    //}

    public class SongStep : Label
    {
        public ImfPlayer ImfPlayer { get; set; }

        public override void _Process(float delta)
        {
            base._Process(delta);
            Text = ImfPlayer.CurrentPacket.ToString();
        }
    }

    public class PlayButton : Button
    {
        public AudioStreamPlayer AudioStreamPlayer { get; set; }

        public override void _GuiInput(InputEvent @event)
        {
            base._GuiInput(@event);
            if (@event.IsPressed())
            {
                if (AudioStreamPlayer.Playing = !AudioStreamPlayer.Playing)
                    SetText("Stop");
                else
                    SetText("Play");
            }
        }
    }
}
