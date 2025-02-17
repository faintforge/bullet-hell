namespace BulletHell {
    public class World {
        private List<Entity> entities = new List<Entity>();
        private ISpatialPartitioner spatialStructure;

        public World() {
            spatialStructure = new SpatialHash(new Vector2(16.0f), 1024);
        }

        public Entity SpawnEntity(EntityFlag flags) {
            Entity entity = new Entity(flags);
            entities.Add(entity);
            return entity;
        }

        public void KillEntity(Entity entity) {
            entities.Remove(entity);
        }

        public void Query(EntityFlag flags, Action<Entity> system) {
            foreach (Entity entity in entities) {
                if (entity.Flags.HasFlag(flags)) {
                    system(entity);
                }
            }
        }

        public List<Entity> SpatialQuery(Vector2 position, float radius) {
            spatialStructure.Clear();
            foreach (Entity entity in entities) {
                spatialStructure.Insert(entity);
            }
            return spatialStructure.Query(position, radius);
        }
    }
}
