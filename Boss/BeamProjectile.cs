namespace BulletHell {
    public class BeamProjectile : Projectile {
        public BeamProjectile(World world) : base(world) {
            Color = Color.HexRGB(0xe8c170);
            Collider = true;
            Friendly = false;
            Damage = 5;
            Pierce = -1;
            lifespan = 0.25f;
        }
    }
}
