namespace BulletHell {
    public class ParticleEmitter : Entity {
        public struct Config {
            public Entity? Parent;
            public int Count;
            public float Time;
            public bool Continuous;

            public Vector2 Size;
            public Color Color;
            public float FinalOpacity;
            public float FinalSize;
            public float RotationMin;
            public float RotationMax;
            public float VelocitySpeedMin;
            public float VelocitySpeedMax;
            public float SpawnAngle;
            public float SpawnRadius;
            public float MinLifespan;
            public float MaxLifespan;
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
                // Transform.Rot = Cfg.Parent.Transform.Rot;
            }

            // Debug.Instance.DrawLine(Transform.Pos, Cfg.SpawnRadius, Transform.Rot - Cfg.SpawnAngle * 0.5f, Color.WHITE);
            // Debug.Instance.DrawLine(Transform.Pos, Cfg.SpawnRadius, Transform.Rot + Cfg.SpawnAngle * 0.5f, Color.WHITE);

            if (!emitting) {
                return;
            }

            float secondsPerSpawn = Cfg.Time / Cfg.Count;
            timer += deltaTime;
            int spawned = 0;
            while (timer >= secondsPerSpawn && spawned < Cfg.Count) {
                timer -= secondsPerSpawn;
                burstTimer += secondsPerSpawn;
                spawned++;

                float offsetAngle = Transform.Rot + Cfg.SpawnAngle * ((float) rng.NextDouble() - 0.5f);
                float velocityAngle = Transform.Rot + Cfg.SpawnAngle * ((float) rng.NextDouble() - 0.5f);
                // Square root of a random number expalined here
                // https://youtu.be/4y_nmpv-9lI?si=FVyN3JUmNekHEuDB
                float distance = Cfg.SpawnRadius * MathF.Sqrt((float) rng.NextDouble());
                Vector2 spawnOffset = Vector2.FromAngle(offsetAngle) * distance;
                Particle particle = world.SpawnEntity<Particle>();
                particle.Transform = new Box() {
                    Pos = Transform.Pos + spawnOffset,
                    Size = Cfg.Size,
                    Rot = Utils.Lerp(Cfg.RotationMin, Cfg.RotationMax, (float) rng.NextDouble()),
                };
                particle.Velocity = Vector2.FromAngle(velocityAngle) * Utils.Lerp(Cfg.VelocitySpeedMin, Cfg.VelocitySpeedMax, (float) rng.NextDouble());
                particle.Color = Cfg.Color;
                particle.FinalSize = Cfg.FinalSize;
                particle.FinalOpacity = Cfg.FinalOpacity;
                particle.Lifespan = Utils.Lerp(Cfg.MinLifespan, Cfg.MaxLifespan, (float) rng.NextDouble());
            }

            if (burstTimer >= Cfg.Time && !Cfg.Continuous) {
                emitting = false;
                burstTimer = 0.0f;
            }
        }

        public void Emit() {
            emitting = true;
        }
    }
}
