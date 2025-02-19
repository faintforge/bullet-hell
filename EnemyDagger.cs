namespace BulletHell {
    public class EnemyDagger : Projectile {
        public EnemyDagger(World world) : base (world) {
            Texture = AssetManager.Instance.GetTexture("enemy_dagger");
            Transform.Size = Texture.Size;
            Friendly = false;
            Damage = 5;
            lifespan = 1.0f;
        }
    }
}
