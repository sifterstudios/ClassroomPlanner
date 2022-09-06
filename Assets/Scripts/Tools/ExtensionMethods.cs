using UnityEngine;

namespace Sifter.Tools
{
    public static class ExtensionMethods
    {
        public static Vector2 ToXZ(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }
    }
}