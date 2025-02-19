namespace BulletHell {
    public class World {
        private List<Entity> entities = new List<Entity>();
        private ISpatialPartitioner spatialStructure;

        private List<Entity> spawnQueue = new List<Entity>();
        private List<Entity> killQueue = new List<Entity>();

        public Camera Camera { get; set; } = new Camera(new Vector2(), new Vector2(), 0.0f);

        public World() {
            spatialStructure = new SpatialHash(new Vector2(16.0f), 1024);
        }

        public T SpawnEntity<T>() where T : Entity {
            T? entity = (T?) Activator.CreateInstance(typeof(T), this);
            if (entity == null) {
                throw new Exception($"Failed to spawn entity of type ${typeof(T)}.");
            }
            spawnQueue.Add(entity);
            return entity;
        }

        public void KillEntity(Entity entity) {
            killQueue.Add(entity);
        }

        public List<Entity> SpatialQuery(Vector2 position, float radius) {
            spatialStructure.Clear();
            foreach (Entity entity in entities) {
                spatialStructure.Insert(entity);
            }
            return spatialStructure.Query(position, radius);
        }

        public List<Entity> SpatialQuery(Box box) {
            spatialStructure.Clear();
            foreach (Entity entity in entities) {
                spatialStructure.Insert(entity);
            }
            return spatialStructure.Query(box);
        }

        public void Update(float deltaTime) {
            List<Entity> spawnQueueCopy = new List<Entity>(spawnQueue);
            spawnQueue.Clear();
            foreach (Entity entity in spawnQueueCopy) {
                entity.OnSpawn();
                entities.Add(entity);
            }

            List<Entity> killQueueCopy = new List<Entity>(killQueue);
            killQueue.Clear();
            foreach (Entity entity in killQueueCopy) {
                entity.OnKill();
                entities.Remove(entity);
            }

            foreach (Entity entity in entities) {
                entity.Update(deltaTime);
            }
        }

        public void OperateOnEntities(Action<Entity> system) {
            foreach (Entity entity in entities) {
                system(entity);
            }
        }

        public void CollisionDetection() {
            foreach (Entity entity in entities) {
                List<Entity> colliding = SpatialQuery(entity.Transform);
                foreach (Entity other in colliding) {
                    if (entity == other) {
                        continue;
                    }
                    entity.OnCollision(other);
                }
            }
        }
    }
}
