using System.Runtime.InteropServices;

namespace BulletHell {
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4 {
        public Vector4 I = new Vector4();
        public Vector4 J = new Vector4();
        public Vector4 K = new Vector4();
        public Vector4 L = new Vector4();

        public Matrix4() {}

        /// <summary>
        /// Create an orthographic projection matrix.
        /// </summary>
        /// <param name="left">Left clipping plane.</param>
        /// <param name="right">Right clipping plane.</param>
        /// <param name="top">Top clipping plane.</param>
        /// <param name="bottom">Bottom clipping plane.</param>
        /// <param name="near">Near clipping plane.</param>
        /// <param name="far">Far clipping plane.</param>
        /// <returns>Orthographic projection matrix.</returns>
        public static Matrix4 OrthographicProjection(float left, float right, float top, float bottom, float near, float far) {
            // http://learnwebgl.brown37.net/08_projections/projections_ortho.html
            Matrix4 mat = new Matrix4();

            float scaleX = 2.0f / (right - left);
            float scaleY = 2.0f / (top - bottom);
            float scaleZ = -2.0f / (far - near);

            float midX = (left + right) / 2.0f;
            float midY = (bottom + top) / 2.0f;
            float midZ = (-near - far) / 2.0f;

            mat.I = new Vector4(scaleX, 0.0f, 0.0f, -midX);
            mat.J = new Vector4(0.0f, scaleY, 0.0f, -midY);
            mat.K = new Vector4(0.0f, 0.0f, scaleZ, -midZ);
            mat.L = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            return mat;
        }

        /// <summary>
        /// Create an inverse orthographic projection matrix.
        /// </summary>
        /// <param name="left">Left clipping plane.</param>
        /// <param name="right">Right clipping plane.</param>
        /// <param name="top">Top clipping plane.</param>
        /// <param name="bottom">Bottom clipping plane.</param>
        /// <param name="near">Near clipping plane.</param>
        /// <param name="far">Far clipping plane.</param>
        /// <returns>Inverse orthographic projection matrix.</returns>
        public static Matrix4 InverseOrthographicProjection(float left, float right, float top, float bottom, float near, float far) {
            // https://en.wikipedia.org/wiki/Orthographic_projection#Geometry
            Matrix4 mat = new Matrix4();

            float scaleX = (right - left) / 2.0f;
            float scaleY = (top - bottom) / 2.0f;
            float scaleZ = (far - near) / -2.0f;

            float midX = (left + right) / 2.0f;
            float midY = (top + bottom) / 2.0f;
            float midZ = -(far + near) / 2.0f;

            mat.I = new Vector4(scaleX, 0.0f, 0.0f, midX);
            mat.J = new Vector4(0.0f, scaleY, 0.0f, midY);
            mat.K = new Vector4(0.0f, 0.0f, scaleZ, midZ);
            mat.L = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            return mat;
        }

        public static Vector4 operator *(Matrix4 a, Vector4 b) {
            return new Vector4(
                    a.I.X * b.X + a.I.Y * b.Y + a.I.Z * b.Z + a.I.W * b.W,
                    a.J.X * b.X + a.J.Y * b.Y + a.J.Z * b.Z + a.J.W * b.W,
                    a.K.X * b.X + a.K.Y * b.Y + a.K.Z * b.Z + a.K.W * b.W,
                    a.L.X * b.X + a.L.Y * b.Y + a.L.Z * b.Z + a.L.W * b.W
                );
        }
    }
}
