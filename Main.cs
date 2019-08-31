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
        AudioStreamGenerator audioStreamGenerator = new AudioStreamGenerator()
        {
            MixRate = 7000,
            BufferLength = 700
        };
        ImfPlayer imfPlayer = new ImfPlayer()
        {
            Opl = Opl,
            Song = ReadImf("WONDERIN_MUS.imf"),
            AudioStreamGenerator = audioStreamGenerator,
            AudioStreamPlayer = new AudioStreamPlayer()
            {
                Stream = audioStreamGenerator,
                VolumeDb = 0.01f
            }
        };

        AddChild(imfPlayer);
        AddChild(imfPlayer.AudioStreamPlayer);
        imfPlayer.AudioStreamPlayer.Play();

            //    AudioStreamSample audioStreamSample = new AudioStreamSample()
            //    {
            //        Data = VSwap.ConcatArrays(
            //    Assets.VSwap.Pages[Assets.VSwap.SoundPage],
            //    Assets.VSwap.Pages[Assets.VSwap.SoundPage + 1]
            //),
            //        Format = AudioStreamSample.FormatEnum.Format8Bits,
            //        MixRate = 7000,
            //        Stereo = false
            //    };
            //    AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer()
            //    {
            //        Stream = audioStreamSample,
            //        VolumeDb = 0.01f
            //    };
            //    AddChild(audioStreamPlayer);
            //    audioStreamPlayer.Play();
        }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
