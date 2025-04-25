namespace BulletHell {
    public abstract class Entity {
        /// <summary>
        /// Box representing the position, size, origin and rotation of the entity in world space.
        /// NOTE: Not a getter or setter because I forgot and making it one now creates A LOT of errors all over the codebase.
        /// </summary>
        public Box Transform = new Box() {
            Pos = new Vector2(),
            Size = new Vector2(1.0f),
            Rot = 0.0f,
            Origin = new Vector2(),
        };

        /// <summary>
        /// Entity can collide with other entities if true, cannot collide if false.
        /// </summary>
        public bool Collider { get; set; } = false;

        protected World world;
        /// <summary>
        /// If true, the entity is still alive in the world.
        /// </summary>
        public bool Alive { get; internal set; }

        /// <summary>
        /// Texture of the entity to be rendered. If it's null a white square will be used.
        /// </summary>
        public Texture? Texture { get; set; } = null;
        /// <summary>
        /// Color to tint the texture when drawn.
        /// </summary>
        public Color Color { get; set; } = Color.WHITE;
        /// <summary>
        /// Render the entity if true, don't render if false.
        /// </summary>
        public bool Render { get; set; } = false;

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
