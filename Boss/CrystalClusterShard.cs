namespace BulletHell {
    public class CrystalClusterShard : Projectile {
        private float timer = 0.0f;

        public CrystalClusterShard(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("crystal_shard");
            Transform.Size = Texture.Size;
            Friendly = false;
            lifespan = 10.0f;
            pierce = -1;
            Damage = 10;
        }

        public override void Update(float deltaTime) {
            timer += deltaTime;
            if (timer < 2.0f) {
                return;
            }

            Transform.Pos += Velocity * deltaTime;
        }
    }
}
