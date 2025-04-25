namespace BulletHell {
    public abstract class Enemy : Entity {
        /// <summary>
        /// Maximum amount of health.
        /// </summary>
        public int MaxHealth { get; set; }
        /// <summary>
        /// Current amount of health.
        /// </summary>
        public int Health { get; set; }
        /// <summary>
        /// Current player being targeted.
        /// </summary>
        public Player? Target { get; set; }

        /// <summary>
        /// Constructor of an enemy.
        /// </summary>
        /// <param name="world">World this enemy belongs to</param>
        public Enemy(World world) : base (world) {
            Render = true;
            Collider = true;
        }

        /// <summary>
        /// Called when the enemy is spawned into the world. Sets the current health to the maximum value.
        /// </summary>
        public override void OnSpawn() {
            Health = MaxHealth;
        }

        /// <summary>
        /// Update the enemy's logic.
        /// </summary>
        /// <param name="deltaTime">Length of the last frame.</param>
        public override void Update(float deltaTime) {
            if (Health <= 0) {
                Kill();
                return;
            }
            AI(deltaTime);
        }

        /// <summary>
        /// Called at the end of the enemy update.
        /// </summary>
        /// <param name="deltaTime">Length of the last frame.</param>
        public virtual void AI(float deltaTime) {}

        /// <summary>
        /// Called when the enemy intersects with a friendly projectile.
        /// </summary>
        /// <param name="projectile">Projectile that hit the enemy.</param>
        /// <param name="damage">Damage taken from the projectile.</param>
        public virtual void OnHit(Projectile projectile, int damage) {}
    }
}
