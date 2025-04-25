namespace BulletHell {
    public class ParticleEmitter : Entity {
        public struct Config {
            /// <summary>
            /// Parent entity to be anchored to.
            /// </summary>
            public Entity? Parent { get; set; }
            /// <summary>
            /// Amount of particles to spawn within the time specified.
            /// </summary>
            public int Count { get; set; }
            /// <summary>
            /// Will the emitter continously spawn particles with no stop.
            /// </summary>
            public bool Continuous { get; set; }
            /// <summary>
            /// Time spent emitting 'count' particles.
            /// </summary>
            public float Time { get; set; }

            /// <summary>
            /// Initial size of the particles.
            /// </summary>
            public Vector2 Size { get; set; }
            /// <summary>
            /// Final size of the particles.
            /// </summary>
            public float FinalSize { get; set; }

            /// <summary>
            /// Color of the particles.
            /// </summary>
            public Color Color { get; set; }
            /// <summary>
            /// Final opacity of the particles color.
            /// </summary>
            public float FinalOpacity { get; set; }


            /// <summary>
            /// Lower bound of the particles rotation.
            /// </summary>
            public float RotationMin { get; set; }
            /// <summary>
            /// Upper bound of the particles rotation.
            /// </summary>
            public float RotationMax { get; set; }

            /// <summary>
            /// Lower bound of the particles velocity.
            /// </summary>
            public float VelocitySpeedMin { get; set; }
            /// <summary>
            /// Upper bound of the particles velocity.
            /// </summary>
            public float VelocitySpeedMax { get; set; }

            /// <summary>
            /// Angle of the cone particles are spawned in.
            /// </summary>
            public float SpawnAngle { get; set; }
            /// <summary>
            /// Radius of the cone particles are spawned in.
            /// </summary>
            public float SpawnRadius { get; set; }

            /// <summary>
            /// Lower bound of particle lifespan.
            /// </summary>
            public float MinLifespan { get; set; }
            /// <summary>
            /// Upper bound of particle lifespan.
            /// </summary>
            public float MaxLifespan { get; set; }
        }

        /// <summary>
        /// Emitter configuration.
        /// </summary>
        public Config Cfg { get; set; }
        private float timer = 0.0f;
        private float burstTimer = 0.0f;
        private bool emitting = true;
        private Random rng = new Random();

        /// <summary>
        /// Create an instance of a particle emitter.
        /// </summary>
        /// <param name="world">World the emtiter belongs to.</param>
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

        /// <summary>
        /// Make the emitter emit if it isn't already.
        /// </summary>
        public void Emit() {
            emitting = true;
        }
    }
}
