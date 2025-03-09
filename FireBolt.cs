namespace BulletHell {
    public class FireBolt : Projectile {
        private ParticleEmitter? emitter;

        public FireBolt(World world) : base (world) {
            Texture = AssetManager.Instance.GetTexture("firebolt");
            Transform.Size = Texture.Size;
            Friendly = true;
            Damage = 5;
            lifespan = 5.0f;
            pierce = 2;
        }

        public override void OnSpawn() {
            emitter = world.SpawnEntity<ParticleEmitter>();
            float angle = MathF.PI / 16.0f;
            Vector2 minVel = Velocity.Normalized().Rotated(angle) * 100.0f;
            Vector2 maxVel = Velocity.Normalized().Rotated(-angle) * 100.0f;
            emitter.Cfg = new ParticleEmitter.Config() {
                Parent = this,
                Time = 1.0f,
                Count = 50,
                Continuous = true,
                Color = Color.HexRGB(0xa53030),
                Size = new Vector2(2.0f),
                FadeTime = 0.0f,
                ShrinkTime = 0.5f,
                RotationMin = Transform.Rot,
                RotationMax = Transform.Rot,
                VelocityMin = minVel,
                VelocityMax = maxVel,
                SpawnRadius = 3.0f,
            };
        }

        public override void OnKill() {
            if (emitter != null) {
                emitter.Kill();
            }
        }
    }
}
