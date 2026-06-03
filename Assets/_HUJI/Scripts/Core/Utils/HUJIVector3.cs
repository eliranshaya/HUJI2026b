using UnityEngine;

namespace HUJI.Gamelogic
{
    public struct HUJIVector3
    {
        public float X;
        public float Y;
        public float Z;

        public HUJIVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public HUJIVector3(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}