namespace BulletHell {
    public class CrystalShard : Projectile {
        public CrystalShard(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("crystal_shard");
            Transform.Size = Texture.Size;
            Friendly = false;
            lifespan = 5.0f;
            pierce = -1;
            Damage = 10;
        }
    }
}
