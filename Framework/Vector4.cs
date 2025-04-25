using System.Runtime.InteropServices;

namespace BulletHell {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4 {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public float X;
        /// <summary>
        /// Y coordinate.
        /// </summary>
        public float Y;
        /// <summary>
        /// Z coordinate.
        /// </summary>
        public float Z;
        /// <summary>
        /// W coordinate.
        /// </summary>
        public float W;

        /// <summary>
        /// Create a 4D vector.
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        /// <param name="z">Z value.</param>
        /// <param name="w">W value.</param>
        public Vector4(float x, float y, float z, float w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}
