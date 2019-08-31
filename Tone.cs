using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPLinGodot
{
    class Tone : Node
    {
        // To convert audio to floats!
        //float = float(int16) / 32767
        //float = float (int8) /127

        float hz = 22050.0f; // less samples to mix
        float phase = 0f;
        float pulse_hz = 440f;
        public AudioStreamPlayer AudioStreamPlayer { get; set; }
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
            AudioStreamGenerator.MixRate = hz;
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
                Vector2 vector2 = GetVector2();
                vector2.Set(foo, foo);
                AudioStreamGeneratorPlayback.PushFrame(vector2);
                Vector2Pool.Push(vector2);
                phase = (phase + increment) % 1f;
            }
        }

        private Stack<Vector2> Vector2Pool = new Stack<Vector2>();
        private Vector2 GetVector2()
        {
            return Vector2Pool.Count > 0 ? Vector2Pool.Pop() : new Vector2(0f, 0f);
        }
    }
}
