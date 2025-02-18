namespace BulletHell {
    public abstract class Entity {
        public Box Transform = new Box() {
            Pos = new Vector2(),
            Size = new Vector2(1.0f),
            Rot = 0.0f,
            Origin = new Vector2(),
        };
        public Texture? Texture = null;
        public Color Color = Color.WHITE;
        public World World { get; private set; }

        public Entity(World world) {
            World = world;
        }

        public virtual void OnSpawn() {}
        public virtual void OnDeath() {}
        public virtual void OnCollision(Entity other) {}
        public virtual void Update(float deltaTime) {}
    }
}
