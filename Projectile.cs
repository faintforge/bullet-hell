namespace BulletHell {
    public abstract class Projectile : Entity {
        public bool Friendly { get; protected set; } = false;
        public int Damage { get; protected set; } = 0;
        public Vector2 Velocity { get; set; } = new Vector2();
        protected float lifespan = -1.0f;

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
            // TODO: Damage the colliding entity and apply knockback.
            // if (!Friendly) {
            //     if (other is Player) {
            //         Console.WriteLine("Hurt him!");
            //     }
            // } else {
            //     if (other is Enemy) {
            //         Console.WriteLine("Hurt it!");
            //     }
            // }
        }
    }
}
