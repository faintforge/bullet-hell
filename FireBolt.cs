namespace BulletHell {
    public class FireBolt : Projectile {
        public FireBolt(World world) : base (world) {
            Transform = new Box() {
                Size = new Vector2(0.5f),
            };
            Friendly = true;
            Color = Color.HexRGB(0xda863e);
            lifespan = 1.0f;
        }
    }
}
