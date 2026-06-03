using UnityEngine;

namespace HUJI.Gamelogic
{
    public struct HUJIVector2
    {
        public float X;
        public float Y;

        public HUJIVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public HUJIVector2(Vector2 vector2)
        {
            X = vector2.x;
            Y = vector2.y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public bool GetIsDefault()
        {
            return Mathf.Approximately(X, 0f) && Mathf.Approximately(Y, 0f);
        }
    }
}