using UnityEngine;

namespace Zero
{
    /// <summary>
    /// Transform的扩展
    /// </summary>
    public static class TransformExtend
    {
        public static void ZSetLocalX(this Transform t, float value)
        {
            var pos = t.localPosition;
            pos.x = value;
            t.localPosition = pos;
        }

        public static void ZSetLocalY(this Transform t, float value)
        {
            var pos = t.localPosition;
            pos.y = value;
            t.localPosition = pos;
        }

        public static void ZSetLocalZ(this Transform t, float value)
        {
            var pos = t.localPosition;
            pos.z = value;
            t.localPosition = pos;
        }

        public static void ZSetX(this Transform t, float value)
        {
            var pos = t.position;
            pos.x = value;
            t.position = pos;
        }

        public static void ZSetY(this Transform t, float value)
        {
            var pos = t.position;
            pos.y = value;
            t.position = pos;
        }

        public static void ZSetZ(this Transform t, float value)
        {
            var pos = t.position;
            pos.z = value;
            t.position = pos;
        }
    }
}
