namespace BulletHell {
    public class Particle : Entity {
        /// <summary>
        /// How long the particle lives before being killed.
        /// </summary>
        public float Lifespan { get; set; }
        /// <summary>
        /// Distance traveled by the particle every second.
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Final size scaler of the particle at the end of the particle lifespan.
        /// </summary>
        public float FinalSize { get; set; }
        private Vector2 startSize;

        /// <summary>
        /// Final state of the opacity at the end of the particle lifespan.
        /// </summary>
        public float FinalOpacity { get; set; }
        private float startOpacity;

        private float timer = 0.0f;

        /// <summary>
        /// Create a particle instance.
        /// </summary>
        /// <param name="world">World to spawn the particle in.</param>
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
            Color newColor = Color;
            newColor.A = Utils.Lerp(startOpacity, FinalOpacity, t);
            Color = newColor;
            Transform.Size = Utils.Lerp(startSize, startSize * FinalSize, t);

            if (timer >= Lifespan) {
                Kill();
            }
        }
    }
}
