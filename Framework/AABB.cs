using System.Diagnostics.CodeAnalysis;

namespace BulletHell {
    public struct AABB {
        /// <summary>
        /// Position of AABB.
        /// </summary>
        public Vector2 Pos { get; set; }
        /// <summary>
        /// Size of AABB.
        /// </summary>
        public Vector2 Size { get; set; }
        /// <summary>
        /// Minimum bounds of AABB.
        /// </summary>
        public Vector2 Min => Pos - Size / 2.0f;
        /// <summary>
        /// Maximum bounds of AABB.
        /// </summary>
        public Vector2 Max => Pos + Size / 2.0f;

        /// <summary>
        /// Checks if two AABBs are intersecting.
        /// </summary>
        /// <param name="other">Other AABB to cheak against.</param>
        /// <returns>Returns true when the two AABBs are intersecting, returns false otherwise.</returns>
        public bool IntersectsAABB(AABB other) {
            return Min.X <= other.Max.X &&
                Max.X >= other.Min.X &&
                Min.Y <= other.Max.Y &&
                Max.Y >= other.Min.Y;
        }

        /// <summary>
        /// Checks if a circle intersects the AABB.
        /// </summary>
        /// <param name="position">Circle position.</param>
        /// <param name="radius">Circle radius.</param>
        /// <returns>returns true when the circle intersects the AABB, returns false otherwise.</returns>
        public bool IntersectsCircle(Vector2 position, float radius) {
            // https://www.jeffreythompson.org/collision-detection/circle-rect.php
            Vector2 min = Min;
            Vector2 max = Max;

            Vector2 test = position;
            if      (position.X < min.X) { test.X = min.X; }
            else if (position.X > max.X) { test.X = max.X; }
            if      (position.Y < min.Y) { test.Y = min.Y; }
            else if (position.Y > max.Y) { test.Y = max.Y; }

            Vector2 distance = position - test;
            return distance.MagnitudeSquared() <= radius * radius;
        }

        /// <summary>
        /// Checks if this AABB fully contains another.
        /// </summary>
        /// <param name="other">Other AABB to be checked against.</param>
        /// <returns>Returns true if this AABB fully contains 'other', returns false otherwise.</returns>
        public bool ContainsAABB(AABB other) {
            return Min.X <= other.Min.X &&
                Max.X >= other.Max.X &&
                Min.Y <= other.Min.Y &&
                Max.Y >= other.Max.Y;
        }

        /// <summary>
        /// Converts an AABB into a box. The origin will be in the center.
        /// </summary>
        /// <param name="aabb">AABB to convert.</param>
        public static explicit operator Box(AABB aabb) {
            return new Box() {
                Origin = new Vector2(),
                Rot = 0.0f,
                Pos = aabb.Pos,
                Size = aabb.Size,
            };
        }

        /// <summary>
        /// Check the equality between two AABBs.
        /// </summary>
        /// <param name="a">left side of equailty check.</param>
        /// <param name="b">Right side of equality check.</param>
        /// <returns>A boolean representing the equailty between the two AABBs.</returns>
        public static bool operator ==(AABB a, AABB b) { return a.Pos == b.Pos && a.Size == b.Size; }

        /// <summary>
        /// Check if two AABBs are not equal.
        /// </summary>
        /// <param name="a">left side of not equal check.</param>
        /// <param name="b">Right side of not equal check.</param>
        /// <returns>A boolean being true when the AABBs aren't equal, false otherwise.</returns>
        public static bool operator !=(AABB a, AABB b) { return a.Pos != b.Pos || a.Size != b.Size; }

        /// <summary>
        /// Check of an object is equal to the AABB.
        /// </summary>
        /// <param name="obj">Object to chack against.</param>
        /// <returns>A boolean representing the equailty between the AABB and obj.</returns>
        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj == null) {
                return false;
            }
            return this == (AABB) obj;
        }

        /// <summary>
        /// Gets the hash code of an AABB.
        /// </summary>
        /// <returns>A 32-bit integer hash of the AABB.</returns>
        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
