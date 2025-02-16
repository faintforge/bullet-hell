namespace BulletHell {
    public class World {
        private List<Entity> entities = new List<Entity>();

        public Entity SpawnEntity(EntityFlag flags) {
            Entity entity = new Entity(flags);
            entities.Add(entity);
            return entity;
        }

        public void KillEntity(Entity entity) {
            entities.Remove(entity);
        }

        public void Query(EntityFlag flags, Action<Entity> system) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i].Flags.HasFlag(flags)) {
                    system(entities[i]);
                }
            }
        }
    }
}
