namespace BulletHell {
    public abstract class Projectile : Entity {
        /// <summary>
        /// If true, do damage to enemies, if false, do damage to player.
        /// </summary>
        public bool Friendly { get; protected set; } = false;
        /// <summary>
        /// Damage dealt when hitting an entity.
        /// </summary>
        public int Damage { get; set; } = 0;
        /// <summary>
        /// Amount of entities that can be hit before being killed.
        /// </summary>
        public int Pierce { get; set; } = 1;
        /// <summary>
        /// Distance traveled per second.
        /// </summary>
        public Vector2 Velocity { get; set; } = new Vector2();

        protected float lifespan = -1.0f;
        private Entity[] alreadyHit = new Entity[8];
        private int alreadyHitIndex = 0;

        /// <summary>
        /// Create an instance of a projectile.
        /// </summary>
        /// <param name="world">World the projectile belongs to.</param>
        public Projectile(World world) : base(world) {
            Render = true;
            Collider = true;
        }

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
                Pierce -= 1;
                enemy.OnHit(this, Damage);
                alreadyHit[alreadyHitIndex] = other;
                alreadyHitIndex = (alreadyHitIndex + 1) % alreadyHit.Length;
            }

            if (!Friendly && other is Player) {
                Player player = (Player) other;
                player.Health -= Damage;
                Pierce -= 1;
                alreadyHit[alreadyHitIndex] = other;
            }

            if (Pierce == 0) {
                Kill();
            }
        }
    }
}
