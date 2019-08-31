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
    public override void _Ready()
    {
        ImfPlayer imfPlayer = new ImfPlayer()
        {
            Opl = new DosBoxOPL(OplType.Opl2),
            Song = ReadImf("WONDERIN_MUS.imf"),
            AudioStreamPlayer = new AudioStreamPlayer()
        };
        AddChild(imfPlayer);
        AddChild(imfPlayer.AudioStreamPlayer);
    }

    //public override void _Process(float delta)
    //{

    //}
}
