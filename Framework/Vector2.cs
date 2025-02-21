using System.Runtime.InteropServices;

namespace BulletHell {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2 {
        public float X { get; set; } = 0.0f;
        public float Y { get; set; } = 0.0f;

        /// <summary>
        /// Create a 2D vector with X and Y initialized to 0.0.
        /// </summary>
        public Vector2() {}

        /// <summary>
        /// Create a 2D vector with X and Y initialized to a scaler value.
        /// </summary>
        /// <param name="scaler">Scaler value.</param>
        public Vector2(float scaler) {
            X = scaler;
            Y = scaler;
        }

        /// <summary>
        /// Create a 2D vector.
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        public Vector2(float x, float y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Element wise multiply two vectors together.
        /// </summary>
        /// <param name="other">Other vector to multiply with.</param>
        /// <returns>Element wise multiplied vector.</returns>
        public Vector2 Mul(Vector2 other) { return new Vector2(X * other.X, Y * other.Y); }

        /// <summary>
        /// Element wise divide two vectors.
        /// </summary>
        /// <param name="devisor">Other vector to divide with.</param>
        /// <returns>Element wise divided vector.</returns>
        public Vector2 Div(Vector2 devisor) { return new Vector2(X / devisor.X, Y / devisor.Y); }

        /// <summary>
        /// Add two vectors together.
        /// </summary>
        /// <param name="other">Other vector to add with.</param>
        /// <returns>Sum of the vectors.</returns>
        public Vector2 Add(Vector2 other) { return new Vector2(X + other.X, Y + other.Y); }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        /// <param name="other">Other vector to subtract with.</param>
        /// <returns>Difference of the vectors.</returns>
        public Vector2 Sub(Vector2 other) { return new Vector2(X - other.X, Y - other.Y); }

        /// <summary>
        /// Scale vector.
        /// </summary>
        /// <param name="scaler">Scaler value.</param>
        /// <returns>Scaled vector.</returns>
        public Vector2 Mul(float scaler) { return new Vector2(X * scaler, Y * scaler); }

        /// <summary>
        /// Divide vector by a scaler value.
        /// Opposite of scaling.
        /// </summary>
        /// <param name="scaler">Scaler value to divide with.</param>
        /// <returns>Divide vector.</returns>
        public Vector2 Div(float scaler) { return new Vector2(X / scaler, Y / scaler); }

        /// <summary>
        /// Scaler add to vector. 
        /// </summary>
        /// <param name="scaler">Scaler value to add.</param>
        /// <returns>Sum of scaler and vector.</returns>
        public Vector2 Add(float scaler) { return new Vector2(X + scaler, Y + scaler); }

        /// <summary>
        /// Scaler subtract from vector. 
        /// </summary>
        /// <param name="scaler">Scaler value to subtract with.</param>
        /// <returns>Difference of vector and scaler.</returns>
        public Vector2 Sub(float scaler) { return new Vector2(X - scaler, Y - scaler); }

        public static Vector2 operator *(Vector2 a, Vector2 b) => a.Mul(b);
        public static Vector2 operator /(Vector2 a, Vector2 b) => a.Div(b);
        public static Vector2 operator +(Vector2 a, Vector2 b) => a.Add(b);
        public static Vector2 operator -(Vector2 a, Vector2 b) => a.Sub(b);
        public static Vector2 operator -(Vector2 a) => new Vector2(-a.X, -a.Y);

        public static Vector2 operator *(Vector2 vec, float scaler) => vec.Mul(scaler);
        public static Vector2 operator /(Vector2 vec, float scaler) => vec.Div(scaler);
        public static Vector2 operator +(Vector2 vec, float scaler) => vec.Add(scaler);
        public static Vector2 operator -(Vector2 vec, float scaler) => vec.Sub(scaler);

        /// <summary>
        /// Get squared magnitude (length) of vector.
        /// </summary>
        /// <returns>Squared magnitude.</returns>
        public float MagnitudeSquared() {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Get magnitude (length) of vector.
        /// </summary>
        /// <returns>Magnitude.</returns>
        public float Magnitude() {
            return MathF.Sqrt(MagnitudeSquared());
        }

        /// <summary>
        /// Calculate the normalized vector.
        /// </summary>
        /// <returns>Normalized vector.</returns>
        public Vector2 Normalized() {
            return this / Magnitude();
        }

        /// <summary>
        /// Normalize this vector, changing its X and Y values.
        /// </summary>
        public void Normalize() {
            this /= Magnitude();
        }

        /// <summary>
        /// Caluclate the rotated value of this vector. 
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Rotated vector.</returns>
        public Vector2 Rotated(float angle) {
            (float sin, float cos) = MathF.SinCos(angle);
            return new Vector2(
                    X * cos - Y * sin,
                    X * sin + Y * cos
                );
        }

        /// <summary>
        /// Rotate this vector.
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        public void Rotate(float angle) {
            this = Rotated(angle);
        }

        /// <summary>
        /// Dot product of two vectors.
        /// </summary>
        /// <param name="other">Other vector.</param>
        /// <returns>Dot product.</returns>
        public float Dot(Vector2 other) {
            return X * other.X + Y * other.Y;
        }
    }
}
