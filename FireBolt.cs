namespace BulletHell {
    public class FireBolt : Projectile {
        public FireBolt(World world) : base (world) {
            Texture = AssetManager.Instance.GetTexture("firebolt");
            Transform.Size = Texture.Size;
            Friendly = true;
            Damage = 5;
            lifespan = 1.0f;
            pierce = 1;
        }
    }
}
