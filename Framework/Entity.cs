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
        protected World world;

        public Entity(World world) {
            this.world = world;
        }

        public void Kill() {
            world.KillEntity(this);
        }

        public virtual void OnSpawn() {}
        public virtual void OnKill() {}
        public virtual void OnCollision(Entity other) {}
        public virtual void Update(float deltaTime) {}
    }
}
