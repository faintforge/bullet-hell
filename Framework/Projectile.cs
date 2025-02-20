namespace BulletHell {
    public abstract class Projectile : Entity {
        public bool Friendly { get; protected set; } = false;
        public int Damage { get; protected set; } = 0;
        public Vector2 Velocity { get; set; } = new Vector2();
        protected float lifespan = -1.0f;
        protected int pierce = 1;
        private Entity[] alreadyHit = new Entity[8];
        private int alreadyHitIndex = 0;

        public Projectile(World world)
            : base(world) {}

        public override void Update(float deltaTime) {
            Transform.Pos += Velocity * deltaTime;

            if (lifespan != -1.0f) {
                lifespan -= deltaTime;
                if (lifespan <= 0.0f) {
                    world.KillEntity(this);
                }
            }
        }

        public override void OnCollision(Entity other) {
            // Prevent double hitting.
            if (alreadyHit.Contains(other)) {
                return;
            }

            if (Friendly && other is Enemy) {
                Enemy enemy = (Enemy) other;
                enemy.Health -= Damage;
                pierce -= 1;
                enemy.OnHit(this, Damage);
                alreadyHit[alreadyHitIndex] = other;
                alreadyHitIndex = (alreadyHitIndex + 1) % alreadyHit.Length;
            }

            if (!Friendly && other is Player) {
                Player player = (Player) other;
                player.Health -= Damage;
                pierce -= 1;
                alreadyHit[alreadyHitIndex] = other;
            }

            if (pierce == 0) {
                Kill();
            }
        }
    }
}
