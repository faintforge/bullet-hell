namespace BulletHell {
    public class Rat : Enemy {
        private float speed = 50.0f;
        public int Damage { get; set; } = 1;
        private float HitCooldown = 0.0f;

        public Rat(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("rat");
            Transform.Size = Texture.Size;
            MaxHealth = 2;
        }

        public override void AI(float deltaTime) {
            if (Target == null) {
                return;
            }
            HitCooldown -= deltaTime;

            Vector2 dir = Target.Transform.Pos - Transform.Pos;
            dir.Normalize();
            Transform.Pos += dir * speed * deltaTime;

            if (Texture != null) {
                Vector2 size = Texture.Size;
                size.X *= MathF.Sign(dir.X);
                Transform.Size = size;
            }
        }

        public override void OnCollision(Entity other) {
            if (other is Player && HitCooldown <= 0.0f) {
                Player player = (Player) other;
                player.Health -= Damage;
                HitCooldown = 0.25f;
            }
        }

        public override void OnKill() {
            ParticleEmitter emitter = world.SpawnEntity<ParticleEmitter>();
            emitter.Cfg = new ParticleEmitter.Config() {
                Parent = this,
                SpawnRadius = 2.0f,
                SpawnAngle = 2.0f * MathF.PI,
                Color = Color.RED,
                Size = new Vector2(2.0f),
                Count = 100,
                Time = 0.0f,
                FinalSize = 0.5f,
                FinalOpacity = 0.0f,
                MinLifespan = 0.0f,
                MaxLifespan = 0.5f,
                VelocitySpeedMax = 100.0f,
            };
            emitter.Kill();

            // Spawn XP on death
            float spawnRadius = 8.0f;
            Random rng = new Random();
            for (int i = 0; i < rng.Next(1, 4); i++) {
                float angle = (float) rng.NextDouble() * 2.0f * MathF.PI;
                float distance = spawnRadius * MathF.Sqrt((float) rng.NextDouble());
                Vector2 pos = Vector2.FromAngle(angle) * distance;
                XpPoint xp = world.SpawnEntity<XpPoint>();
                xp.Transform.Pos = Transform.Pos + pos;
            }
        }
    }
}
