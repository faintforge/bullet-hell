namespace BulletHell {
    public class CrystalClusterShard : Projectile {
        private float timer = 0.0f;

        public CrystalClusterShard(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("crystal_shard");
            Transform.Size = Texture.Size;
            Friendly = false;
            lifespan = 3.0f;
            Pierce = -1;
            Damage = 2;
        }

        public override void Update(float deltaTime) {
            timer += deltaTime;
            if (timer < 2.0f) {
                return;
            }

            base.Update(deltaTime);
        }
    }
}
