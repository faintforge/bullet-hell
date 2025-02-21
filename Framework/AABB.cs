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
    }
}
