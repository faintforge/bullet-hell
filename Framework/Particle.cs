namespace BulletHell {
    public class Particle : Entity {
        public Vector2 Velocity { get; set; }
        public float Lifespan { get; set; }
        public float FinalSize { get; set; }
        public float FinalOpacity { get; set; }
        private Vector2 startSize;
        private float timer = 0.0f;
        private float startOpacity;

        public Particle(World world) : base(world) {
            Render = true;
        }

        public override void OnSpawn() {
            startSize = Transform.Size;
            startOpacity = Color.A;
        }

        public override void Update(float deltaTime) {
            Transform.Pos += Velocity * deltaTime;

            timer += deltaTime;
            float t = timer / Lifespan;
            Color.A = Utils.Lerp(startOpacity, FinalOpacity, t);
            Transform.Size = Utils.Lerp(startSize, startSize * FinalSize, t);

            if (timer >= Lifespan) {
                Kill();
            }
        }
    }
}
