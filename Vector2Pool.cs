using Godot;
using System.Collections.Generic;

namespace OPLinGodot
{
    public static class Vector2Pool
    {
        private static readonly Queue<Vector2> Queue = new Queue<Vector2>();

        public static Vector2 Get()
        {
            return Queue.Count > 0 ? Queue.Dequeue() : new Vector2(0f, 0f);
        }

        public static void Return(Vector2 vector2)
        {
            vector2.Set(0f, 0f);
            Queue.Enqueue(vector2);
        }
    }
}
