using Godot;
using NScumm.Core.Audio.OPL;
using NScumm.Core.Audio.OPL.DosBox;
using OPL;
using OPLinGodot;
using System.IO;
using static OPL.Imf;

public class Main : Control
{
    public static IOpl Opl = new DosBoxOPL(OplType.Opl3);
    public static ImfPlayer ImfPlayer;
    public static AdlPlayer AdlPlayer;
    public static Adl Adl;
    public static ImfPacket[] Song;

    public override void _Ready()
    {
        using (FileStream file = new FileStream("WONDERIN_MUS.imf", FileMode.Open))
            Song = ReadImf(file);
        ImfPlayer = new ImfPlayer()
        {
            Opl = Opl,
            Song = Song,
            AudioStreamPlayer = new AudioStreamPlayer(),
            Music = true,
        };
        AddChild(ImfPlayer);
        AddChild(ImfPlayer.AudioStreamPlayer);

        using (FileStream file = new FileStream(
            "GETAMMOSND.adl"
            //"GETMACHINESND.adl"
            , FileMode.Open))
            Adl = new Adl(file);

        AdlPlayer = new AdlPlayer
        {
            Opl = Opl,
            //Adl = Adl
        };
        AddChild(AdlPlayer);

        Button button = new PlayButton
        {
            Text = "Sound"
        };
        AddChild(button);

        SongStep songStep = new SongStep();
        AddChild(songStep);
    }

    //public override void _Process(float delta)
    //{

    //}

    public class SongStep : Label
    {
        public override void _Process(float delta)
        {
            base._Process(delta);
            //Text = ImfPlayer.CurrentPacket.ToString();
            Text = AdlPlayer?.CurrentNote.ToString() ?? "null";
        }
    }

    public class PlayButton : Button
    {
        public override void _GuiInput(InputEvent @event)
        {
            base._GuiInput(@event);
            if (@event.IsPressed())
            {
                //if (AudioStreamPlayer.Playing = !AudioStreamPlayer.Playing)
                //    SetText("Stop");
                //else
                //    SetText("Play");
                //if (Main.ImfPlayer.Song == null)
                //    Main.ImfPlayer.Song = Imf;
                //else
                //    Main.ImfPlayer.Song = null;

                Main.AdlPlayer.Adl = Adl;
            }
        }
    }
}
