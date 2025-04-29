using System.Diagnostics;

namespace BulletHell {
    public class Quadtree : ISpatialPartitioner {
        private struct Bucket {
            public AABB BoundingBox { get; set; }
            public Entity Entity { get; set; }
        }

        private struct Node {
            private Quadtree quadtree { get; }
            public List<Bucket> buckets = new List<Bucket>();
            public AABB Area { get; }

            public int[] Children { get; set; } = new int[4]{-1, -1, -1, -1};

            public Node(Quadtree quadtree, AABB area) {
                this.quadtree = quadtree;
                this.Area = area;
            }

            public bool Insert(Bucket bucket, int depth) {
                // Don't insert buckets that don't fit.
                if (!Area.ContainsAABB(bucket.BoundingBox)) {
                    // Color c = Color.HSV(depth * 36.0f, 0.75f, 1.0f);
                    // c.A = 0.1f;
                    // Debug.Instance.DrawBox((Box) Area, c);
                    // Debug.Instance.DrawBox((Box) bucket.BoundingBox, Color.WHITE);
                    return false;
                }

                // Don't go beyond the recursion depth.
                if (depth >= quadtree.MaxDepth) {
                    buckets.Add(bucket);
                    return true;
                }

                // If the node has children insert into them. If the bucket
                // doesn't fit inside, insert into this one.
                if (Children[0] != -1) {
                    for (int i = 0; i < Children.Length; i++) {
                        if (quadtree.GetNodeIndex(Children[i]).Insert(bucket, depth + 1)) {
                            return true;
                        }
                    }

                    // If no child can fit this bucket insert into this one.
                    buckets.Add(bucket);
                    return true;
                }

                // If we've reached max capacity split into 4 subregions and try
                // to insert into them. If a bucket doesn't fit inside any
                // children keep it in this one.
                if (buckets.Count >= quadtree.MaxCapacity && Children[0] == -1) {
                    Children[0] = quadtree.GetNewNode(new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(-4.0f, 4.0f),
                            Size = Area.Size / 2.0f,
                        });
                    Children[1] = quadtree.GetNewNode(new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(4.0f, 4.0f),
                            Size = Area.Size / 2.0f,
                        });
                    Children[2] = quadtree.GetNewNode(new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(-4.0f, -4.0f),
                            Size = Area.Size / 2.0f,
                        });
                    Children[3] = quadtree.GetNewNode(new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(4.0f, -4.0f),
                            Size = Area.Size / 2.0f,
                        });

                    // Add first into this node, then try to insert all buckets
                    // into its children. If a child can contain it, cache it
                    // for later removal.
                    buckets.Add(bucket);
                    List<Bucket> bucketsInsertedIntoChildren = new List<Bucket>();
                    foreach (Bucket _bucket in buckets) {
                        bool insertedIntoChild = false;
                        for (int i = 0; i < Children.Length; i++) {
                            if (quadtree.GetNodeIndex(Children[i]).Insert(_bucket, depth + 1)) {
                                insertedIntoChild = true;
                                break;
                            }
                        }
                        if (insertedIntoChild) {
                            bucketsInsertedIntoChildren.Add(_bucket);
                        }
                    }
                    // Console.WriteLine($"Before inserting into children: {buckets.Count}");
                    buckets = buckets.Except(bucketsInsertedIntoChildren).ToList();
                    // Console.WriteLine($"After inserting into children: {buckets.Count}");
                    // Console.WriteLine($"{bucketsInsertedIntoChildren.Count}");

                    return true;
                }

                buckets.Add(bucket);
                return true;
            }

            public List<Entity> Query(Vector2 position, float radius) {
                if (!Area.IntersectsCircle(position, radius)) {
                    return new List<Entity>();
                }

                List<Entity> result = new List<Entity>();
                if (Children[0] != -1) {
                    for (int i = 0; i < Children.Length; i++) {
                        result.AddRange(quadtree.GetNodeIndex(Children[i]).Query(position, radius));
                    }
                }

                foreach (Bucket bucket in buckets) {
                    Profiler.Instance.Start("Intersect Box Test");
                    if (bucket.Entity.Transform.IntersectsCircle(position, radius)) {
                        result.Add(bucket.Entity);
                    }
                    Profiler.Instance.End();
                }

                return result;
            }

            public List<Entity> Query(AABB boundingBox, Box box) {
                Profiler.Instance.Start("Is inside area");
                if (!boundingBox.IntersectsAABB(Area)) {
                    Profiler.Instance.End();
                    return new List<Entity>();
                }
                Profiler.Instance.End();

                List<Entity> result = new List<Entity>();
                if (Children[0] != -1) {
                    for (int i = 0; i < Children.Length; i++) {
                        result.AddRange(quadtree.GetNodeIndex(Children[i]).Query(boundingBox, box));
                    }
                }

                foreach (Bucket bucket in buckets) {
                    Profiler.Instance.Start("Check bounding boxes");
                    if (bucket.BoundingBox == boundingBox) {
                        Profiler.Instance.End();
                        continue;
                    }
                    Profiler.Instance.End();

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

                return result;
            }

            [Conditional("DEBUG")]
            public void DebugDraw() {
                Debug.Instance.DrawBoxOutline((Box) Area, Color.HexRGBA(0xffffff80));
                if (Children[0] != -1) {
                    for (int i = 0; i < Children.Length; i++) {
                        quadtree.GetNodeIndex(Children[i]).DebugDraw();
                    }
                }
            }
        }

        internal int MaxDepth { get; }
        internal int MaxCapacity { get; }
        private int root;
        private List<Node> nodePool = new List<Node>();
        private int currentNode = 0;

        /// <summary>
        /// Create a quadtree instance.
        /// </summary>
        /// <param name="area">Area covered by the quadtree.</param>
        /// <param name="maxDepth">Maximum amount of subdivisions possible.</param>
        /// <param name="maxCapacity">Maximum amount of objects in a cell before subdividing.</param>
        public Quadtree(AABB area, int maxDepth, int maxCapacity) {
            MaxDepth = maxDepth;
            MaxCapacity = maxCapacity;
            root = GetNewNode(area);
        }

        /// <summary>
        /// Insert an entity into the quadtree.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public void Insert(Entity entity) {
            nodePool[root].Insert(new Bucket() {
                    Entity = entity,
                    BoundingBox = entity.Transform.GetBoundingAABB(),
                }, 0);
        }

        /// <summary>
        /// Query quadtree for enemies intersecting a circle.
        /// </summary>
        /// <param name="position">Center point of circle.</param>
        /// <param name="radius">Radius of circle.</param>
        /// <returns>List of entities intersecting the circle.</returns>
        public List<Entity> Query(Vector2 position, float radius) {
            return nodePool[root].Query(position, radius);
        }

        /// <summary>
        /// Query quadtree for enemies intersecting a rectangle.
        /// </summary>
        /// <param name="box">Rectangle to check against.</param>
        /// <returns>List of entities intersecting rectangle.</returns>
        public List<Entity> Query(Box box) {
            return nodePool[root].Query(box.GetBoundingAABB(), box);
        }

        /// <summary>
        /// Remove all entities from the quadtree.
        /// </summary>
        public void Clear() {
            currentNode = 0;
            root = GetNewNode(nodePool[root].Area);
        }

        /// <summary>
        /// Draw wireframe of quadtree.
        /// </summary>
        [Conditional("DEBUG")]
        public void DebugDraw() {
            nodePool[root].DebugDraw();
        }

        private int GetNewNode(AABB aabb) {
            if (currentNode == nodePool.Count) {
                nodePool.Add(new Node(this, aabb));
            }
            nodePool[currentNode] = new Node(this, aabb);
            return currentNode++;
        }

        private Node GetNodeIndex(int index) {
            return nodePool[index];
        }
    }
}
