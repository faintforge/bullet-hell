namespace BulletHell {
    public class SpatialHash : ISpatialPartitioner {
        private struct Bucket {
            public AABB BoundingBox { get; set; }
            public Entity Entity { get; set; }
        }

        private Vector2 cellSize;
        private List<Bucket>[] buckets;
        private int bucketCount;

        /// <summary>
        /// Create a spatial hash data structure.
        /// </summary>
        /// <param name="cellSize">Size of a single cell containing entities.</param>
        /// <param name="bucketCount">Amount of buckets in the internal map. A higher number means fewer collisions.</param>
        public SpatialHash(Vector2 cellSize, int bucketCount) {
            buckets = new List<Bucket>[bucketCount];
            this.bucketCount = bucketCount;
            this.cellSize = cellSize;
        }

        /// <summary>
        /// Insert an entity into the spatial structure.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public void Insert(Entity entity) {
            AABB boundingBox = entity.Transform.GetBoundingAABB();
            Vector2 min = boundingBox.Min / cellSize;
            Vector2 max = boundingBox.Max / cellSize;
            int minX = (int) MathF.Round(min.X);
            int minY = (int) MathF.Round(min.Y);
            int maxX = (int) MathF.Round(max.X);
            int maxY = (int) MathF.Round(max.Y);

            for (int y = minY; y <= maxY; y++) {
                for (int x = minX; x <= maxX; x++) {
                    long hash = HashPosition(x, y);
                    int index = (int) hash % bucketCount;
                    // Apparently mod can return negative numbers here so we
                    // have to make sure it's positive.
                    if (index < 0) {
                        index += bucketCount;
                    }

                    if (buckets[index] == null) {
                        buckets[index] = new List<Bucket>();
                    }
                    buckets[index].Add(new Bucket() {
                            BoundingBox = boundingBox,
                            Entity = entity,
                        });
                }
            }
        }

        /// <summary>
        /// Query for entities lying within a certain radius of a position.
        /// </summary>
        /// <param name="position">Center point of query.</param>
        /// <param name="radius">Radius of query.</param>
        /// <returns>List of entities within the specified query circle.</returns>
        public List<Entity> Query(Vector2 position, float radius) {
            Vector2 min = (position - radius) / cellSize;
            Vector2 max = (position + radius) / cellSize;
            int minX = (int) MathF.Round(min.X);
            int minY = (int) MathF.Round(min.Y);
            int maxX = (int) MathF.Round(max.X);
            int maxY = (int) MathF.Round(max.Y);

            // List<Entity> result = new List<Entity>();
            HashSet<Entity> result = new HashSet<Entity>();
            for (int y = minY; y <= maxY; y++) {
                for (int x = minX; x <= maxX; x++) {
                    long hash = HashPosition(x, y);
                    int index = (int) hash % bucketCount;
                    if (index < 0) {
                        index += bucketCount;
                    }

                    if (buckets[index] == null) {
                        continue;
                    }

                    foreach (Bucket bucket in buckets[index]) {
                        if (bucket.Entity.Transform.IntersectsCircle(position, radius)) {
                            result.Add(bucket.Entity);
                        }
                    }
                }
            }
            return result.ToList();
        }

        /// <summary>
        /// Qeury for entities lying within a certain box region.
        /// </summary>
        /// <param name="box">Query region.</param>
        /// <returns>List of entities within the specified box region.</returns>
        public List<Entity> Query(Box box) {
            AABB boundingBox = box.GetBoundingAABB();
            Vector2 min = boundingBox.Min / cellSize;
            Vector2 max = boundingBox.Max / cellSize;
            int minX = (int) MathF.Round(min.X);
            int minY = (int) MathF.Round(min.Y);
            int maxX = (int) MathF.Round(max.X);
            int maxY = (int) MathF.Round(max.Y);

            HashSet<Entity> result = new HashSet<Entity>();
            for (int y = minY; y <= maxY; y++) {
                for (int x = minX; x <= maxX; x++) {
                    long hash = HashPosition(x, y);
                    int index = (int) hash % bucketCount;
                    if (index < 0) {
                        index += bucketCount;
                    }

                    if (buckets[index] == null) {
                        continue;
                    }

                    foreach (Bucket bucket in buckets[index]) {
                        Profiler.Instance.Start("Intersect AABB Test");
                        if (!bucket.BoundingBox.IntersectsAABB(boundingBox)) {
                            Profiler.Instance.End();
                            continue;
                        }
                        Profiler.Instance.End();

                        Profiler.Instance.Start("Intersect Box Test");
                        if (bucket.Entity.Transform.IntersectsBox(box)) {
                            result.Add(bucket.Entity);
                        }
                        Profiler.Instance.End();
                    }
                }
            }
            return result.ToList();
        }

        /// <summary>
        /// Clear the spatial structure of entities.
        /// </summary>
        public void Clear() {
            buckets = new List<Bucket>[bucketCount];
        }

        private long HashPosition(int x, int y) {
            // https://stackoverflow.com/questions/6943493/hash-table-with-64-bit-values-as-key
            long key = ((long) x << 32) | (long) y;
            key = (~key) + (key << 21); // key = (key << 21) - key - 1;
            key = key ^ (key >>> 24);
            key = (key + (key << 3)) + (key << 8); // key * 265
            key = key ^ (key >>> 14);
            key = (key + (key << 2)) + (key << 4); // key * 21
            key = key ^ (key >>> 28);
            key = key + (key << 31);
            return key;
        }
    }
}
