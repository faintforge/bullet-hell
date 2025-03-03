namespace BulletHell {
    public struct Box {
        public Vector2 Pos { get; set; }
        public Vector2 Size { get; set; }
        public float Rot { get; set; }
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Gets the vertices of the box in a specific order.
        /// [0] = Top left
        /// [1] = Top right
        /// [2] = Bottom right
        /// [3] = Bottom left
        /// </summary>
        /// <returns>Array of four vertices.</returns>
        public Vector2[] GetVertices() {
            Vector2[] verts = new Vector2[4] {
                new Vector2(-0.5f,  0.5f),
                new Vector2( 0.5f,  0.5f),
                new Vector2( 0.5f, -0.5f),
                new Vector2(-0.5f, -0.5f),
            };
            for (int i = 0; i < verts.Length; i++) {
                verts[i] -= Origin / 2.0f;
                verts[i] *= Size;
                verts[i].Rotate(Rot);
                verts[i] += Pos;
            }
            return verts;
        }

        /// <summary>
        /// Gets an axis aligned bounding box from a potentially unaligned box.
        /// </summary>
        /// <returns>Axis aligned bounding box.</returns>
        public AABB GetBoundingAABB() {
            Vector2[] verts = GetVertices();
            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            foreach (Vector2 vert in verts) {
                min.X = MathF.Min(min.X, vert.X);
                min.Y = MathF.Min(min.Y, vert.Y);
                max.X = MathF.Max(max.X, vert.X);
                max.Y = MathF.Max(max.Y, vert.Y);
            }
            Vector2 size = max - min;
            Vector2 pos = min + size / 2.0f;
            return new AABB() {
                Pos = pos,
                Size = size,
            };
        }

        /// <summary>
        /// Gets the center point the box in world space.
        /// </summary>
        /// <returns>Center coordinates.</returns>
        public Vector2 Center() {
            return Pos - (Size * Origin / 2.0f).Rotated(-Rot);
        }

        /// <summary>
        /// Checks if a point lies within the box.
        /// </summary>
        /// <param name="point">Point to check against.</param>
        /// <returns>Whether the point lies within the box or not.</returns>
        public bool IntersectsPoint(Vector2 point) {
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

        /// <summary>
        /// Checks if a circle intersects the box.
        /// </summary>
        /// <param name="position">Circle position.</param>
        /// <param name="radius">Circle radius.</param>
        /// <returns>Whether the circle intersects the box or not.</returns>
        public bool IntersectsCircle(Vector2 position, float radius) {
            // https://www.jeffreythompson.org/collision-detection/circle-rect.php
            Vector2 center = Center();
            Vector2 relPos = position - center;
            relPos.Rotate(-Rot);
            relPos += center;

            Vector2 min = center - Size / 2.0f;
            Vector2 max = center + Size / 2.0f;

            Vector2 test = relPos;
            if      (relPos.X < min.X) { test.X = min.X; }
            else if (relPos.X > max.X) { test.X = max.X; }
            if      (relPos.Y < min.Y) { test.Y = min.Y; }
            else if (relPos.Y > max.Y) { test.Y = max.Y; }

            Vector2 distance = relPos - test;
            return distance.MagnitudeSquared() <= radius * radius;
        }

        /// <summary>
        /// Checks if two boxes are intersecting eachother using the separating axis theorem.
        /// </summary>
        /// <param name="other">The other box to check against.</param>
        /// <returns>Whether the boxes are intersecting or not.</returns>
        public bool IntersectsBox(Box other) {
            Vector2[] aVertices = GetVertices();
            Vector2[] bVertices = other.GetVertices();

            // Find normals / axes to check against.
            Vector2[] normals = new Vector2[4];
            for (int i = 0; i < 2; i++) {
                Vector2 edge = aVertices[(i + 1) % aVertices.Length] - aVertices[i];
                Vector2 normal = new Vector2(-edge.Y, edge.X);
                normals[i] = normal;
            }
            for (int i = 0; i < 2; i++) {
                Vector2 edge = bVertices[(i + 1) % bVertices.Length] - bVertices[i];
                Vector2 normal = new Vector2(-edge.Y, edge.X);
                normals[2 + i] = normal;
            }

            // https://programmerart.weebly.com/separating-axis-theorem.html
            foreach (Vector2 normal in normals) {
                float aMin = float.PositiveInfinity;
                float aMax = float.NegativeInfinity;
                foreach (Vector2 vertex in aVertices) {
                    float dot = normal.Dot(vertex);
                    aMin = MathF.Min(aMin, dot);
                    aMax = MathF.Max(aMax, dot);
                }

                float bMin = float.PositiveInfinity;
                float bMax = float.NegativeInfinity;
                foreach (Vector2 vertex in bVertices) {
                    float dot = normal.Dot(vertex);
                    bMin = MathF.Min(bMin, dot);
                    bMax = MathF.Max(bMax, dot);
                }

                if (!((aMin < bMax && aMin > bMin) ||
                    (bMin < aMax && bMin > aMin))) {
                    return false;
                }
            }
            return true;
        }
    }
}
