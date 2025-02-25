using SDL2;

namespace BulletHell {
    public class Player : Entity {
        private float speed = 100.0f;
        private float shootTimer = 0.0f;
        private float shootDelay = 0.1f;
        public int MaxHealth { get; set; } = 100;
        public int Health { get; set; }

        public Player(World world) : base(world) {
            Render = true;
            Texture = AssetManager.Instance.GetTexture("player");
            float asepctRatio = Texture.Size.X / Texture.Size.Y;
            Transform.Size = Texture.Size;
        }

        public override void OnSpawn() {
            Health = MaxHealth;
        }

        public override void Update(float deltaTime) {
            Vector2 vel = new Vector2();
            vel.X -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_a));
            vel.X += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_d));
            vel.Y -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_s));
            vel.Y += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_w));
            if (vel.MagnitudeSquared() != 0.0f) {
                vel.Normalize();
            }
            vel *= speed;
            Transform.Pos += vel * deltaTime;

            // Lerp camera to player position
            world.Camera.Position = new Vector2(
                    world.Camera.Position.X + (Transform.Pos.X - world.Camera.Position.X) * deltaTime * 5.0f,
                    world.Camera.Position.Y + (Transform.Pos.Y - world.Camera.Position.Y) * deltaTime * 5.0f);

            shootTimer += deltaTime;
            if (Input.Instance.GetButton(MouseButton.Left) && shootTimer >= shootDelay) {
                shootTimer = 0.0f;
                Projectile proj = world.SpawnEntity<FireBolt>();
                proj.Transform.Pos = Transform.Pos;
                Vector2 mousePos = world.Camera.ScreenToWorldSpace(Input.Instance.MousePosition);
                Vector2 direction = (mousePos - Transform.Pos).Normalized();
                proj.Velocity = direction * 200.0f;
                proj.Transform.Rot = MathF.Atan2(direction.Y, direction.X) - MathF.PI / 2.0f;
            }
        }
    }
}
