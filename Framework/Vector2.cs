using System.Runtime.InteropServices;

namespace BulletHell {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2 {
        public float X { get; set; } = 0.0f;
        public float Y { get; set; } = 0.0f;

        public Vector2() {}
        public Vector2(float scaler) {
            X = scaler;
            Y = scaler;
        }
        public Vector2(float x, float y) {
            X = x;
            Y = y;
        }

        public Vector2 Mul(Vector2 other) { return new Vector2(X * other.X, Y * other.Y); }
        public Vector2 Div(Vector2 other) { return new Vector2(X / other.X, Y / other.Y); }
        public Vector2 Add(Vector2 other) { return new Vector2(X + other.X, Y + other.Y); }
        public Vector2 Sub(Vector2 other) { return new Vector2(X - other.X, Y - other.Y); }

        public Vector2 Mul(float scaler) { return new Vector2(X * scaler, Y * scaler); }
        public Vector2 Div(float scaler) { return new Vector2(X / scaler, Y / scaler); }
        public Vector2 Add(float scaler) { return new Vector2(X + scaler, Y + scaler); }
        public Vector2 Sub(float scaler) { return new Vector2(X - scaler, Y - scaler); }

        public static Vector2 operator *(Vector2 a, Vector2 b) => a.Mul(b);
        public static Vector2 operator /(Vector2 a, Vector2 b) => a.Div(b);
        public static Vector2 operator +(Vector2 a, Vector2 b) => a.Add(b);
        public static Vector2 operator -(Vector2 a, Vector2 b) => a.Sub(b);

        public static Vector2 operator *(Vector2 vec, float scaler) => vec.Mul(scaler);
        public static Vector2 operator /(Vector2 vec, float scaler) => vec.Div(scaler);
        public static Vector2 operator +(Vector2 vec, float scaler) => vec.Add(scaler);
        public static Vector2 operator -(Vector2 vec, float scaler) => vec.Sub(scaler);

        public float MagnitudeSquared() {
            return X * X + Y * Y;
        }

        public float Magnitude() {
            return (float) Math.Sqrt(MagnitudeSquared());
        }

        public Vector2 Normalized() {
            return this / Magnitude();
        }

        public void Normalize() {
            this /= Magnitude();
        }

        public Vector2 Rotated(float angle) {
            return new Vector2(
                     X * (float) Math.Cos(angle) + Y * (float) Math.Sin(angle),
                    -X * (float) Math.Sin(angle) + Y * (float) Math.Cos(angle)
                );
        }
        public void Rotate(float angle) {
            this = Rotated(angle);
        }

        public float Dot(Vector2 other) {
            return X * other.X + Y * other.Y;
        }
    }
}
