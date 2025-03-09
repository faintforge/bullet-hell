namespace BulletHell {
    public class ParticleEmitter : Entity {
        public struct Config {
            public Entity? Parent;
            public int Count;
            public float Time;
            public bool Continuous;

            public Vector2 Size;
            public Color Color;
            public float FadeTime;
            public float ShrinkTime;
            public float RotationMin;
            public float RotationMax;
            public Vector2 VelocityMin;
            public Vector2 VelocityMax;
            public float SpawnRadius;
        }

        public Config Cfg { get; set; }
        private float timer = 0.0f;
        private float burstTimer = 0.0f;
        private bool emitting = true;
        private Random rng = new Random();

        public ParticleEmitter(World world) : base(world) {
            Render = false;
        }

        public override void Update(float deltaTime) {
            if (Cfg.Parent != null) {
                Transform.Pos = Cfg.Parent.Transform.Pos;
            }

            if (!emitting) {
                return;
            }

            float secondsPerSpawn = Cfg.Time / Cfg.Count;
            timer += deltaTime;
            while (timer >= secondsPerSpawn) {
                timer -= secondsPerSpawn;
                burstTimer += secondsPerSpawn;

                float angle = MathF.PI * 2.0f * (float) rng.NextDouble();
                float distance = Cfg.SpawnRadius * (float) rng.NextDouble();
                Vector2 spawnOffset = Vector2.FromAngle(angle) * distance;
                Particle particle = world.SpawnEntity<Particle>();
                particle.Transform = new Box() {
                    Pos = Transform.Pos + spawnOffset,
                    Size = Cfg.Size,
                    Rot = Cfg.RotationMin + (Cfg.RotationMax - Cfg.RotationMin) * (float) rng.NextDouble(),
                };
                particle.Velocity = Cfg.VelocityMin + (Cfg.VelocityMax - Cfg.VelocityMin) * (float) rng.NextDouble();
                particle.Color = Cfg.Color;
                particle.ShrinkTime = Cfg.ShrinkTime;
                particle.FadeTime = Cfg.FadeTime;
            }

            if (burstTimer > Cfg.Time && !Cfg.Continuous) {
                emitting = false;
                burstTimer = 0.0f;
            }
        }

        public void Emit() {
            emitting = true;
        }
    }
}
