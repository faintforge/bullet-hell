namespace BulletHell {
    public interface ISpatialPartitioner {
        /// <summary>
        /// Insert an entity into the spatial structure.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public void Insert(Entity entity);

        /// <summary>
        /// Query for entities lying within a certain radius of a position.
        /// </summary>
        /// <param name="position">Center point of query.</param>
        /// <param name="radius">Radius of query.</param>
        /// <returns>List of entities within the specified query circle.</returns>
        public List<Entity> Query(Vector2 position, float radius);

        /// <summary>
        /// Qeury for entities lying within a certain box region.
        /// </summary>
        /// <param name="box">Query region.</param>
        /// <returns>List of entities within the specified box region.</returns>
        public List<Entity> Query(Box box);

        /// <summary>
        /// Clear the spatial structure of entities.
        /// </summary>
        public void Clear();
    }
}
