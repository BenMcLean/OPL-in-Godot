using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPLinGodot
{
    /// <summary>
    /// This class just generates a tone.
    /// 
    /// It is a direct port of the AudioStreamGenerator demo at https://github.com/godotengine/godot-demo-projects/tree/master/audio/generator
    /// 
    /// The following example code shows how to get this class to work:
    /// Tone tone = new Tone()
    /// {
    ///     AudioStreamPlayer = new AudioStreamPlayer()
    /// };
    /// AddChild(tone.AudioStreamPlayer);
    /// AddChild(tone);
    /// </summary>
    class Tone : Node
    {
        // To convert audio to floats!
        //float = float(int16) / 32767
        //float = float (int8) /127

        readonly float hz = 22050f; // less samples to mix
        float phase = 0f;
        readonly float pulse_hz = 440f;

        private AudioStreamPlayer audioStreamPlayer;
        public AudioStreamPlayer AudioStreamPlayer
        {
            get
            {
                return audioStreamPlayer;
            }
            set
            {
                audioStreamPlayer = value;
                value.Stream = new AudioStreamGenerator()
                {
                    MixRate = hz
                };
            }
        }

        public AudioStreamGeneratorPlayback AudioStreamGeneratorPlayback
        {
            get
            {
                return (AudioStreamGeneratorPlayback)AudioStreamPlayer?.GetStreamPlayback();
            }
        }

        public AudioStreamGenerator AudioStreamGenerator
        {
            get
            {
                return (AudioStreamGenerator)AudioStreamPlayer?.GetStream();
            }
        }

        public override void _Ready()
        {
            base._Ready();
            FillBuffer();
            AudioStreamPlayer.Play();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            FillBuffer();
        }

        public void FillBuffer()
        {
            float increment = (1.0f / (hz / pulse_hz));
            int toFill = AudioStreamGeneratorPlayback.GetFramesAvailable();
            for (uint i = 0; i < toFill; i++)
            {
                float foo = Godot.Mathf.Sin(phase * (Godot.Mathf.Pi * 2f));
                Vector2.Set(foo, foo);
                AudioStreamGeneratorPlayback.PushFrame(Vector2);
                phase = (phase + increment) % 1f;
            }
        }

        private Vector2 Vector2 = new Vector2(0f, 0f);
    }
}
