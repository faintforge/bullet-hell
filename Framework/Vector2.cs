using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace BulletHell {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2 {
        /// <summary>
        /// X component.
        /// </summary>
        public float X { get; set; } = 0.0f;
        /// <summary>
        /// Y component.
        /// </summary>
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

        public static Vector2 FromAngle(float radians) {
            (float sin, float cos) = MathF.SinCos(radians);
            return new Vector2(cos, sin);
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

        /// <summary>
        /// Element wise multiply two vectors together.
        /// </summary>
        /// <param name="a">Left side of expression.</param>
        /// <param name="b">Right side of expression.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Vector2 operator *(Vector2 a, Vector2 b) => a.Mul(b);

        /// <summary>
        /// Element wise division between two vectors.
        /// </summary>
        /// <param name="a">Dividend.</param>
        /// <param name="b">Divisor.</param>
        /// <returns>Quotient of the operation.</returns>
        public static Vector2 operator /(Vector2 a, Vector2 b) => a.Div(b);

        /// <summary>
        /// Element wise additin of two vectors.
        /// </summary>
        /// <param name="a">Left side of expression.</param>
        /// <param name="b">Right side of expression.</param>
        /// <returns>Sum of the addition.</returns>
        public static Vector2 operator +(Vector2 a, Vector2 b) => a.Add(b);

        /// <summary>
        /// Element wise subtraction of two vectors.
        /// </summary>
        /// <param name="a">Left side of expression.</param>
        /// <param name="b">Right side of expression.</param>
        /// <returns>Difference of the subtraction.</returns>
        public static Vector2 operator -(Vector2 a, Vector2 b) => a.Sub(b);

        /// <summary>
        /// Negation operator.
        /// </summary>
        /// <param name="a">Vector to negate.</param>
        /// <returns>Inverted vector.</returns>
        public static Vector2 operator -(Vector2 a) => new Vector2(-a.X, -a.Y);

        /// <summary>
        /// Scaler multiplication.
        /// </summary>
        /// <param name="vec">Vector.</param>
        /// <param name="scaler">Scaler.</param>
        /// <returns>Scaled vector.</returns>
        public static Vector2 operator *(Vector2 vec, float scaler) => vec.Mul(scaler);

        /// <summary>
        /// Scaler division.
        /// </summary>
        /// <param name="vec">Vector.</param>
        /// <param name="scaler">Scaler.</param>
        /// <returns>Scaler divided vector.</returns>
        public static Vector2 operator /(Vector2 vec, float scaler) => vec.Div(scaler);

        /// <summary>
        /// Scaler addition.
        /// </summary>
        /// <param name="vec">Vector.</param>
        /// <param name="scaler">Scaler.</param>
        /// <returns>Scaler added vector.</returns>
        public static Vector2 operator +(Vector2 vec, float scaler) => vec.Add(scaler);

        /// <summary>
        /// Scaler subtraction.
        /// </summary>
        /// <param name="vec">Vector.</param>
        /// <param name="scaler">Scaler.</param>
        /// <returns>Scaler subtracted vector.</returns>
        public static Vector2 operator -(Vector2 vec, float scaler) => vec.Sub(scaler);

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">Left side of expression.</param>
        /// <param name="b">Right side of expression.</param>
        /// <returns>True if a and b are equal, false if not.</returns>
        public static bool operator ==(Vector2 a, Vector2 b) { return a.X == b.X && a.Y == b.Y; }

        /// <summary>
        /// Not equal operator.
        /// </summary>
        /// <param name="a">Left side of expression.</param>
        /// <param name="b">Right side of expression.</param>
        /// <returns>True if a and b are not equal, false if not.</returns>
        public static bool operator !=(Vector2 a, Vector2 b) { return a.X != b.X || a.Y != b.Y; }

        /// <summary>
        /// Equality check with objects.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>The equality between the object and vector.</returns>
        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj == null) {
                return false;
            }
            return this == (Vector2) obj;
        }

        /// <summary>
        /// Gets 32-bit integer hash from object.
        /// </summary>
        /// <returns>32-bit integer hash.</returns>
        public override int GetHashCode() { return base.GetHashCode(); }

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

        /// <summary>
        /// Converts vector into a string representation.
        /// </summary>
        /// <returns>String representation of vector.</returns>
        public override string ToString() {
            return $"({X}, {Y})";
        }
    }
}
