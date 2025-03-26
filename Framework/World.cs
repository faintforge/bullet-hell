namespace BulletHell {
    public class World {
        public List<Entity> Entities { get; } = new List<Entity>();
        private ISpatialPartitioner spatialStructure;

        private List<Entity> spawnQueue = new List<Entity>();
        private List<Entity> killQueue = new List<Entity>();

        public Camera Camera { get; set; } = new Camera(new Vector2(), new Vector2(), 0.0f);

        /// <summary>
        /// Create a world containing entities.
        /// </summary>
        public World() {
            // spatialStructure = new SpatialHash(new Vector2(16.0f), 1024);
            // Depth of 9 means 8192 / 2^9 = 16
            spatialStructure = new Quadtree(new AABB() {
                    Size = new Vector2(8192.0f),
                }, 9, 4);
        }

        /// <summary>
        /// Spawn an entity.
        /// </summary>
        /// <typeparam name="T">Type of entity to spawn.</typeparam>
        /// <returns>Entity spawned.</returns>
        /// <exception cref="Exception">If entity failed to be spawned.</exception>
        public T SpawnEntity<T>() where T : Entity {
            T? entity = (T?) Activator.CreateInstance(typeof(T), this);
            if (entity == null) {
                throw new Exception($"Failed to spawn entity of type ${typeof(T)}.");
            }
            spawnQueue.Add(entity);
            return entity;
        }

        /// <summary>
        /// Kill an entity.
        /// </summary>
        /// <param name="entity">Entity to kill.</param>
        public void KillEntity(Entity entity) {
            killQueue.Add(entity);
        }

        /// <summary>
        /// Query for entities lying within a circle.
        /// </summary>
        /// <param name="position">Circle position.</param>
        /// <param name="radius">Circle radius.</param>
        /// <returns>List of entities within the circle.</returns>
        public List<Entity> SpatialQuery(Vector2 position, float radius) {
            if (spatialStructure == null) {
                return new List<Entity>();
            }
            return spatialStructure.Query(position, radius);
        }

        /// <summary>
        /// Query for entities lying within a box. 
        /// </summary>
        /// <param name="box">Box region.</param>
        /// <returns>List of entities within box.</returns>
        public List<Entity> SpatialQuery(Box box) {
            if (spatialStructure == null) {
                return new List<Entity>();
            }
            return spatialStructure.Query(box);
        }

        /// <summary>
        /// Take a step in the world simulation.
        /// </summary>
        /// <param name="deltaTime">Time between frames.</param>
        public void Update(float deltaTime) {
            Profiler.Instance.Start("Entity Update");

            List<Entity> spawnQueueCopy = new List<Entity>(spawnQueue);
            spawnQueue.Clear();
            foreach (Entity entity in spawnQueueCopy) {
                entity.OnSpawn();
                Entities.Add(entity);
            }

            foreach (Entity entity in Entities) {
                entity.Update(deltaTime);
            }

            List<Entity> killQueueCopy = new List<Entity>(killQueue);
            killQueue.Clear();
            foreach (Entity entity in killQueueCopy) {
                entity.OnKill();
                Entities.Remove(entity);
            }

            Profiler.Instance.End();

            Profiler.Instance.Start("Build Spatial Structure");
            spatialStructure.Clear();
            foreach (Entity entity in Entities) {
                if (!entity.Collider) {
                    continue;
                }
                spatialStructure.Insert(entity);
            }
            Profiler.Instance.End();

            Profiler.Instance.Start("Collision Detection");
            CollisionDetection();
            Profiler.Instance.End();

            // Profiler.Instance.Start("Quadtree Debug Draw");
            // if (spatialStructure is Quadtree) {
            //     ((Quadtree) spatialStructure).DebugDraw();
            // }
            // Profiler.Instance.End();
        }

        private void CollisionDetection() {
            foreach (Entity entity in Entities) {
                if (!entity.Collider) {
                    continue;
                }

                Profiler.Instance.Start("Entity Spatial Query");
                List<Entity> colliding = SpatialQuery(entity.Transform);
                Profiler.Instance.End();
                foreach (Entity other in colliding) {
                    if (entity == other) {
                        continue;
                    }

                    if (entity is Projectile && other is Projectile) {
                        continue;
                    }

                    entity.OnCollision(other);
                }
            }
        }

        /// <summary>
        /// Perform some operation on all entities.
        /// </summary>
        /// <param name="system">Operation to perform on every entity.</param>
        public void OperateOnEntities(Action<Entity> system) {
            foreach (Entity entity in Entities) {
                system(entity);
            }
        }
    }
}
