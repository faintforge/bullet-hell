namespace BulletHell {
    public abstract class Entity {
        public Box Transform = new Box() {
            Pos = new Vector2(),
            Size = new Vector2(1.0f),
            Rot = 0.0f,
            Origin = new Vector2(),
        };
        public Texture? Texture { get; set; } = null;
        public Color Color { get; set; } = Color.WHITE;
        public bool Render { get; set; } = false;
        protected World world;
        public bool Collider { get; set; } = false;
        public bool Alive { get; internal set; }

        /// <summary>
        /// Create an entity beloning to a world.
        /// </summary>
        /// <param name="world">Owning world.</param>
        public Entity(World world) {
            this.world = world;
        }

        /// <summary>
        /// Kills an entity deleting it from its world.
        /// </summary>
        public void Kill() {
            world.KillEntity(this);
        }

        /// <summary>
        /// Called when this entity type is spawned.
        /// </summary>
        public virtual void OnSpawn() {}

        /// <summary>
        /// Called when this entity type is killed.
        /// </summary>
        public virtual void OnKill() {}

        /// <summary>
        /// Called when an entity collides with this entity type.
        /// </summary>
        /// <param name="other">The colliding entity.</param>
        public virtual void OnCollision(Entity other) {}

        /// <summary>
        /// Called when the world takes a simulation step.
        /// </summary>
        /// <param name="deltaTime">The amount of time passed in the previous frame.</param>
        public virtual void Update(float deltaTime) {}
    }
}
