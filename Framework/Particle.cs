namespace BulletHell {
    public class Particle : Entity {
        public Vector2 Velocity { get; set; }
        public float ShrinkTime { get; set; }
        public float FadeTime { get; set; }
        private Vector2 originalSize;
        private float timer = 0.0f;

        public Particle(World world) : base(world) {
            Render = true;
        }

        public override void OnSpawn() {
            originalSize = Transform.Size;
        }

        public override void Update(float deltaTime) {
            Transform.Pos += Velocity * deltaTime;

            timer += deltaTime;
            if (ShrinkTime != 0.0f) {
                Transform.Size = originalSize * (1.0f - timer / ShrinkTime);
                if (timer >= ShrinkTime) {
                    Kill();
                }
            }

            if (FadeTime != 0.0f) {
                Color.A = 1.0f - timer / FadeTime;
                if (timer >= FadeTime) {
                    Kill();
                }
            }
        }
    }
}
