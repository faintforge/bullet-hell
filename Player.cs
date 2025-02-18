using SDL2;

namespace BulletHell {
    public class Player : Entity {
        private float speed = 25.0f;

        public Player(World world)
            : base(world) {}

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
            World.Camera.Position = new Vector2(
                    World.Camera.Position.X + (Transform.Pos.X - World.Camera.Position.X) * deltaTime * 5.0f,
                    World.Camera.Position.Y + (Transform.Pos.Y - World.Camera.Position.Y) * deltaTime * 5.0f);
        }
    }
}
