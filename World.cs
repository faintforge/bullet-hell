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

        public void Update(float deltaTime) {
            foreach (Entity entity in spawnQueue) {
                entities.Add(entity);
            }
            spawnQueue.Clear();

            foreach (Entity entity in killQueue) {
                entities.Remove(entity);
            }
            killQueue.Clear();

            foreach (Entity entity in entities) {
                entity.Update(deltaTime);
            }
        }

        public void OperateOnEntities(Action<Entity> system) {
            foreach (Entity entity in entities) {
                system(entity);
            }
        }
    }
}
