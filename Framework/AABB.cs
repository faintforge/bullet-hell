using System.Diagnostics.CodeAnalysis;

namespace BulletHell {
    public struct AABB {
        public Vector2 Pos { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Min => Pos - Size / 2.0f;
        public Vector2 Max => Pos + Size / 2.0f;

        public bool IntersectsAABB(AABB other) {
            return Min.X <= other.Max.X &&
                Max.X >= other.Min.X &&
                Min.Y <= other.Max.Y &&
                Max.Y >= other.Min.Y;
        }

        public static explicit operator Box(AABB aabb) {
            return new Box() {
                Origin = new Vector2(),
                Rot = 0.0f,
                Pos = aabb.Pos,
                Size = aabb.Size,
            };
        }

        public static bool operator ==(AABB a, AABB b) { return a.Pos == b.Pos && a.Size == b.Size; }
        public static bool operator !=(AABB a, AABB b) { return a.Pos != b.Pos || a.Size != b.Size; }
        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj == null) {
                return false;
            }
            return this == (AABB) obj;
        }
        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
