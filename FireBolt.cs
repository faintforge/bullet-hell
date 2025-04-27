namespace BulletHell {
    public class FireBolt : Projectile {
        private ParticleEmitter? emitter;

        public FireBolt(World world) : base (world) {
            Texture = AssetManager.Instance.GetTexture("firebolt");
            Transform.Size = Texture.Size;
            Friendly = true;
            Damage = 10;
            lifespan = 5.0f;
            Pierce = 1;
        }

        public override void OnSpawn() {
            emitter = world.SpawnEntity<ParticleEmitter>();
            float angle = MathF.PI / 16.0f;
            Vector2 minVel = Velocity.Normalized().Rotated(angle) * 100.0f;
            Vector2 maxVel = Velocity.Normalized().Rotated(-angle) * 100.0f;
            emitter.Cfg = new ParticleEmitter.Config() {
                Parent = this,
                Time = 1.0f,
                Count = 100,
                Continuous = true,
                Color = Color.HexRGB(0xa53030),
                Size = new Vector2(2.0f),
                FinalOpacity = 0.0f,
                FinalSize = 0.0f,
                MinLifespan = 0.25f,
                MaxLifespan = 0.25f,
                RotationMin = Transform.Rot,
                RotationMax = Transform.Rot,
                VelocitySpeedMax = 100.0f,
                VelocitySpeedMin = 10.0f,
                SpawnAngle = MathF.PI / 2.0f,
                SpawnRadius = 4.0f,
            };
            emitter.Transform.Rot = Transform.Rot - MathF.PI / 2.0f;
        }

        public override void OnKill() {
            if (emitter != null) {
                emitter.Kill();
            }
        }
    }
}
