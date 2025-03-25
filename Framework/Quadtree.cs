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

            public void Insert(Bucket bucket, int depth) {
                if (depth >= quadtree.MaxDepth) {
                    buckets.Add(bucket);
                    return;
                }

                if (!bucket.BoundingBox.IntersectsAABB(Area)) {
                    return;
                }

                if (Children[0] != -1) {
                    for (int i = 0; i < Children.Length; i++) {
                        quadtree.GetNodeIndex(Children[i]).Insert(bucket, depth + 1);
                    }
                    return;
                }

                if (buckets.Count >= quadtree.MaxCapacity) {
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

                    buckets.Add(bucket);
                    foreach (Bucket _bucket in buckets) {
                        for (int i = 0; i < Children.Length; i++) {
                            quadtree.GetNodeIndex(Children[i]).Insert(_bucket, depth + 1);
                        }
                    }
                    buckets.Clear();

                    return;
                }

                buckets.Add(bucket);
            }

            public HashSet<Entity> Query(Vector2 position, float radius) {
                if (!Area.IntersectsCircle(position, radius)) {
                    return new HashSet<Entity>();
                }

                HashSet<Entity> result = new HashSet<Entity>();
                if (Children[0] != -1) {
                    for (int i = 0; i < Children.Length; i++) {
                        result.UnionWith(quadtree.GetNodeIndex(Children[i]).Query(position, radius));
                    }
                } else {
                    foreach (Bucket bucket in buckets) {
                        // Profiler.Instance.Start("Intersect AABB Test");
                        // if (!bucket.BoundingBox.IntersectsCircle(position, radius)) {
                        //     Profiler.Instance.End();
                        //     continue;
                        // }
                        // Profiler.Instance.End();

                        Profiler.Instance.Start("Intersect Box Test");
                        if (bucket.Entity.Transform.IntersectsCircle(position, radius)) {
                            result.Add(bucket.Entity);
                        }
                        Profiler.Instance.End();
                    }
                }
                return result;
            }

            public HashSet<Entity> Query(AABB boundingBox, Box box) {
                if (!boundingBox.IntersectsAABB(Area)) {
                    return new HashSet<Entity>();
                }

                HashSet<Entity> result = new HashSet<Entity>();
                if (Children[0] != -1) {
                    for (int i = 0; i < Children.Length; i++) {
                        result.UnionWith(quadtree.GetNodeIndex(Children[i]).Query(boundingBox, box));
                    }
                } else {
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

        public int MaxDepth { get; }
        public int MaxCapacity { get; }
        private int root;
        private List<Node> nodePool = new List<Node>();
        private int currentNode = 0;

        public Quadtree(AABB area, int maxDepth, int maxCapacity) {
            MaxDepth = maxDepth;
            MaxCapacity = maxCapacity;
            root = GetNewNode(area);
        }

        public void Insert(Entity entity) {
            nodePool[root].Insert(new Bucket() {
                    Entity = entity,
                    BoundingBox = entity.Transform.GetBoundingAABB(),
                }, 0);
        }

        public HashSet<Entity> Query(Vector2 position, float radius) {
            return nodePool[root].Query(position, radius);
        }

        public HashSet<Entity> Query(Box box) {
            return nodePool[root].Query(box.GetBoundingAABB(), box);
        }

        public void Clear() {
            currentNode = 0;
            root = GetNewNode(nodePool[root].Area);
        }

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
