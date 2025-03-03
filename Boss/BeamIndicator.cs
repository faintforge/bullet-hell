namespace BulletHell {
    public class BeamIndicator : Entity {
        private float timer = 0.0f;

        public BeamIndicator(World world) : base(world) {
            Transform.Size = new Vector2(1.0f, 16.0f);
            Collider = false;
            Render = true;
            Color = Color.WHITE;
        }

        public override void Update(float deltaTime) {
            timer += deltaTime;
            if (timer >= 1.0f) {
                Kill();
                BeamProjectile proj = world.SpawnEntity<BeamProjectile>();
                proj.Transform = Transform;
                // proj.Transform.Rot = Transform.Rot + MathF.PI / 2.0f;
                // proj.Velocity = new Vector2(100.0f, 0.0f).Rotated(Transform.Rot);
            }
        }
    }
}
