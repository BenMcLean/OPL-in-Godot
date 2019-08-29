using Godot;
using NScumm.Core.Audio.OPL;
using NScumm.Core.Audio.OPL.DosBox;
using System;

public class Main : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        IOpl opl = new DosBoxOPL(OplType.Opl3);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
