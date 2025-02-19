namespace BulletHell {
    public abstract class Enemy : Entity {
        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public Enemy(World world)
            : base (world) {}

        public override void OnSpawn() {
            Health = MaxHealth;
        }

        public override void Update(float deltaTime) {
            if (Health <= 0) {
                Kill();
                return;
            }
            AI(deltaTime);
        }

        public virtual void AI(float detlaTime) {}
        public virtual void OnHit(Projectile projectile, int damage) {}
    }
}
