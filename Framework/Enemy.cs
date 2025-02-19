namespace BulletHell {
    public abstract class Enemy : Entity {
        public int Health { get; set; }

        public Enemy(World world)
            : base (world) {}

        public override void Update(float deltaTime) {
            AI(deltaTime);
        }

        public virtual void AI(float detlaTime) {}
        public virtual void OnHit(Projectile projectile, int damage) {}
    }
}
