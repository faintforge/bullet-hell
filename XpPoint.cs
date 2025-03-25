namespace BulletHell {
    public class XpPoint : Entity {
        public XpPoint(World world) : base(world) {
            Render = true;
            Transform = new Box() {
                Size = new Vector2(2),
            };
            Collider = true;
            Color = Color.HexRGB(0xa4dddb);
        }

        public override void OnKill() {
            ParticleEmitter emitter = world.SpawnEntity<ParticleEmitter>();
            emitter.Cfg = new ParticleEmitter.Config() {
                Parent = this,
                SpawnRadius = 1.0f,
                SpawnAngle = 2.0f * MathF.PI,
                Color = Color.HexRGB(0xa4dddb),
                Size = new Vector2(2.0f),
                Count = 25,
                Time = 0.0f,
                FinalSize = 0.5f,
                FinalOpacity = 0.0f,
                MinLifespan = 0.0f,
                MaxLifespan = 0.5f,
                VelocitySpeedMax = 20.0f,
            };
            emitter.Kill();
        }
    }
}
