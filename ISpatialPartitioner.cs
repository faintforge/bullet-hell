namespace BulletHell {
    public interface ISpatialPartitioner {
        public void Insert(Entity entity);
        public List<Entity> Query(Vector2 position, float radius);
        public void Clear();
    }
}
