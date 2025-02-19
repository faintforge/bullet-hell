namespace BulletHell {
    public class EnemyDagger : Projectile {
        public EnemyDagger(World world) : base (world) {
            Transform.Size = new Vector2(1.0f, 0.1f);
            Friendly = false;
            Damage = 5;
            lifespan = 1.0f;
        }
    }
}
