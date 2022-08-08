using System;
using UnityEngine;

namespace RPGProject.Saving
{
    [Serializable]
    public struct SerializableVector3
    {
        float x, y, z;

        public SerializableVector3(Vector3 _vector3)
        {
            x = _vector3.x;
            y = _vector3.y;
            z = _vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
