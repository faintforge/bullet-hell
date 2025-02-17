namespace BulletHell {
    public struct Box {
        public Vector2 Pos { get; set; }
        public Vector2 Size { get; set; }
        public float Rot { get; set; }
        public Vector2 Origin { get; set; }

        public Vector2[] GetVertices() {
            Vector2[] verts = new Vector2[4] {
                new Vector2(-0.5f,  0.5f),
                new Vector2( 0.5f,  0.5f),
                new Vector2(-0.5f, -0.5f),
                new Vector2( 0.5f, -0.5f),
            };
            for (int i = 0; i < verts.Length; i++) {
                verts[i] -= Origin / 2.0f;
                verts[i].Rotate(Rot);
                verts[i] *= Size;
                verts[i] += Pos;
            }
            return verts;
        }

        public Box GetBoundingBox() {
            Vector2[] verts = GetVertices();
            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new Vector2();
            foreach (Vector2 vert in verts) {
                min.X = Math.Min(min.X, vert.X);
                min.Y = Math.Min(min.Y, vert.Y);
                max.X = Math.Max(max.X, vert.X);
                max.Y = Math.Max(max.Y, vert.Y);
            }
            Vector2 size = max - min;
            Vector2 pos = min + size / 2.0f;
            return new Box() {
                Origin = new Vector2(),
                Pos = pos,
                Size = size,
            };
        }

        public Vector2 Center() {
            return Pos - ((Size * Origin) / 2.0f).Rotated(-Rot);
        }

        public bool ContainsPoint(Vector2 point) {
            Vector2 center = Center();
            Vector2 relPoint = point - center;
            relPoint.Rotate(-Rot);
            relPoint += center;

            Vector2 min = center - Size / 2.0f;
            Vector2 max = center + Size / 2.0f;
            return relPoint.X >= min.X &&
                relPoint.X <= max.X &&
                relPoint.Y >= min.Y &&
                relPoint.Y <= max.Y;
        }
    }
}
