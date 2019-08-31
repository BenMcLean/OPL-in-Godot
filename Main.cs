using Godot;
using NScumm.Core.Audio.OPL;
using NScumm.Core.Audio.OPL.DosBox;
using OPLinGodot;
using System;
using System.Collections.Generic;
using System.IO;
using static OPL.Imf;

public class Main : Control
{
    //public ImfPacket[] imf;
    public static IOpl Opl { get; set; } = new DosBoxOPL(OplType.Opl3);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Tone tone = new Tone()
        {
            AudioStreamPlayer = new AudioStreamPlayer()
            {
                Stream = new AudioStreamGenerator()
            }
        };

        AddChild(tone.AudioStreamPlayer);
        AddChild(tone);

        //ImfPlayer imfPlayer = new ImfPlayer()
        //{
        //    Opl = Opl,
        //    Song = ReadImf("WONDERIN_MUS.imf"),
        //    AudioStreamGenerator = audioStreamGenerator,
        //    AudioStreamPlayer = new AudioStreamPlayer()
        //    {
        //        Stream = audioStreamGenerator,
        //        VolumeDb = 0.01f
        //    }
        //};

        //AddChild(imfPlayer);
        //AddChild(imfPlayer.AudioStreamPlayer);
        //imfPlayer.AudioStreamPlayer.Play();

        }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
