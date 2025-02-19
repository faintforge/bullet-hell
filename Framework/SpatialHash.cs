namespace BulletHell {
    public class SpatialHash : ISpatialPartitioner {
        private Vector2 cellSize;
        private List<Entity>[] buckets;
        private int bucketCount;

        /// <summary>
        /// Create a spatial hash data structure.
        /// </summary>
        /// <param name="cellSize">Size of a single cell containing entities.</param>
        /// <param name="bucketCount">Amount of buckets in the internal map. A higher number means fewer collisions.</param>
        public SpatialHash(Vector2 cellSize, int bucketCount) {
            buckets = new List<Entity>[bucketCount];
            this.bucketCount = bucketCount;
            this.cellSize = cellSize;
        }

        /// <summary>
        /// Insert an entity into the spatial structure.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public void Insert(Entity entity) {
            Box boundingBox = entity.Transform.GetBoundingBox();
            Vector2 min = (boundingBox.Pos - boundingBox.Size / 2.0f) / cellSize;
            Vector2 max = (boundingBox.Pos + boundingBox.Size / 2.0f) / cellSize;
            int minX = (int) Math.Round(min.X);
            int minY = (int) Math.Round(min.Y);
            int maxX = (int) Math.Round(max.X);
            int maxY = (int) Math.Round(max.Y);

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
                        buckets[index] = new List<Entity>();
                    }
                    buckets[index].Add(entity);
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
            int minX = (int) Math.Round(min.X);
            int minY = (int) Math.Round(min.Y);
            int maxX = (int) Math.Round(max.X);
            int maxY = (int) Math.Round(max.Y);

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

                    foreach (Entity entity in buckets[index]) {
                        if (entity.Transform.IntersectsCircle(position, radius)) {
                            result.Add(entity);
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
            Box boundingBox = box.GetBoundingBox();

            Vector2 min = (boundingBox.Pos - boundingBox.Size / 2.0f) / cellSize;
            Vector2 max = (boundingBox.Pos + boundingBox.Size / 2.0f) / cellSize;
            int minX = (int) Math.Round(min.X);
            int minY = (int) Math.Round(min.Y);
            int maxX = (int) Math.Round(max.X);
            int maxY = (int) Math.Round(max.Y);

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

                    foreach (Entity entity in buckets[index]) {
                        if (entity.Transform.IntersectsBox(box)) {
                            result.Add(entity);
                        }
                    }
                }
            }
            return result.ToList();
        }

        /// <summary>
        /// Clear the spatial structure of entities.
        /// </summary>
        public void Clear() {
            buckets = new List<Entity>[bucketCount];
        }

        private long HashPosition(int x, int y) {
            // https://stackoverflow.com/questions/6943493/hash-table-with-64-bit-values-as-key
            long key = ((long) x << 32) & y;
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
