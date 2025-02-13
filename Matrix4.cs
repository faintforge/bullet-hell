using System.Runtime.InteropServices;

namespace BulletHell {
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4 {
        public float[] I = new float[4];
        public float[] J = new float[4];
        public float[] K = new float[4];
        public float[] L = new float[4];

        public Matrix4() {}

        // http://learnwebgl.brown37.net/08_projections/projections_ortho.html
        public static Matrix4 OrthographicProjection(float left, float right, float top, float bottom, float near, float far) {
            Matrix4 mat = new Matrix4();

            float midX = (left + right) / 2.0f;
            float midY = (bottom + top) / 2.0f;
            float midZ = (-near - far) / 2.0f;

            float scaleX = 2.0f / (right - left);
            float scaleY = 2.0f / (top - bottom);
            float scaleZ = 2.0f / (far - near);

            mat.I = new float[4]{scaleX, 0.0f, 0.0f, -midX};
            mat.J = new float[4]{0.0f, scaleY, 0.0f, -midY};
            mat.K = new float[4]{0.0f, 0.0f, scaleZ, -midZ};
            mat.L = new float[4]{0.0f, 0.0f, 0.0f, 1.0f};

            return mat;
        }
    }
}
