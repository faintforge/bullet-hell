using System.Diagnostics;

namespace BulletHell {
    public class Quadtree : ISpatialPartitioner {
        private struct Bucket {
            public AABB BoundingBox { get; set; }
            public Entity Entity { get; set; }
        }

        private class Node {
            private Quadtree quadtree { get; }
            private List<Bucket> buckets = new List<Bucket>();
            public AABB Area { get; }

            Node? nw; // North West
            Node? ne; // North East
            Node? sw; // South West
            Node? se; // South East

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

                if (nw != null && ne != null && sw != null && se != null) {
                    nw.Insert(bucket, depth + 1);
                    ne.Insert(bucket, depth + 1);
                    sw.Insert(bucket, depth + 1);
                    se.Insert(bucket, depth + 1);
                    return;
                }

                if (buckets.Count >= quadtree.MaxCapacity) {
                    nw = new Node(quadtree, new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(-4.0f, 4.0f),
                            Size = Area.Size / 2.0f,
                        });
                    ne = new Node(quadtree, new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(4.0f, 4.0f),
                            Size = Area.Size / 2.0f,
                        });
                    sw = new Node(quadtree, new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(-4.0f, -4.0f),
                            Size = Area.Size / 2.0f,
                        });
                    se = new Node(quadtree, new AABB() {
                            Pos = Area.Pos + Area.Size / new Vector2(4.0f, -4.0f),
                            Size = Area.Size / 2.0f,
                        });

                    buckets.Add(bucket);
                    foreach (Bucket _bucket in buckets) {
                        nw.Insert(_bucket, depth + 1);
                        ne.Insert(_bucket, depth + 1);
                        sw.Insert(_bucket, depth + 1);
                        se.Insert(_bucket, depth + 1);
                    }
                    buckets.Clear();

                    return;
                }

                buckets.Add(bucket);
            }

            public List<Entity> Query(Vector2 position, float radius) {
                throw new NotImplementedException();
            }

            public List<Entity> Query(AABB boundingBox, Box box) {
                if (!boundingBox.IntersectsAABB(Area)) {
                    return new List<Entity>();
                }
                if (nw != null && ne != null && sw != null && se != null) {
                    List<Entity> result = new List<Entity>();
                    result.AddRange(nw.Query(boundingBox, box));
                    result.AddRange(ne.Query(boundingBox, box));
                    result.AddRange(sw.Query(boundingBox, box));
                    result.AddRange(se.Query(boundingBox, box));
                    return result;
                } else {
                HashSet<Entity> result = new HashSet<Entity>();
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
                return result.ToList();
                }
            }

            [Conditional("DEBUG")]
            public void DebugDraw() {
                Debug.Instance.DrawBoxOutline((Box) Area, Color.HexRGBA(0xffffff80));
                if (nw != null && ne != null && sw != null && se != null) {
                    nw.DebugDraw();
                    ne.DebugDraw();
                    sw.DebugDraw();
                    se.DebugDraw();
                }
            }
        }

        public int MaxDepth { get; }
        public int MaxCapacity { get; }
        private Node root;

        public Quadtree(AABB area, int maxDepth, int maxCapacity) {
            MaxDepth = maxDepth;
            MaxCapacity = maxCapacity;
            root = new Node(this, area);
        }

        public void Insert(Entity entity) {
            root.Insert(new Bucket() {
                    Entity = entity,
                    BoundingBox = entity.Transform.GetBoundingAABB(),
                }, 0);
        }

        public List<Entity> Query(Vector2 position, float radius) {
            return root.Query(position, radius);
        }

        public List<Entity> Query(Box box) {
            return root.Query(box.GetBoundingAABB(), box);
        }

        public void Clear() {
            root = new Node(this, root.Area);
        }

        [Conditional("DEBUG")]
        public void DebugDraw() {
            root.DebugDraw();
        }
    }
}
