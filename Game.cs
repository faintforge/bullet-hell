using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    public class Game {
        private Window window;
        private Renderer renderer;
        private Camera cam;
        private World world = new World();

        private Entity player;

        public Game(Window window, Renderer renderer) {
            this.window = window;
            this.renderer = renderer;
            cam = new Camera(window.Size, new Vector2(), 50.0f);

            for (int y = 0; y < 64; y++) {
                for (int x = 0; x < 64; x++) {
                    Entity ent = world.SpawnEntity(EntityFlag.Renderable);
                    ent.Transform.Pos = new Vector2(x, y) * 2.0f;
                }
            }

            player = world.SpawnEntity(EntityFlag.Player | EntityFlag.Renderable);
            player.Color = Color.HexRGB(0xfcba03);
        }

        public void Run(float dt) {
            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            world.Query(EntityFlag.Player, (entity) => {
                Vector2 vel = new Vector2();
                vel.X -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_a));
                vel.X += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_d));
                vel.Y -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_s));
                vel.Y += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_w));
                if (vel.MagnitudeSquared() != 0.0f) {
                    vel.Normalize();
                }
                vel *= entity.Speed;
                entity.Transform.Pos += vel * dt;

                // Lerp camera to player position
                cam.Position = new Vector2(
                        cam.Position.X + (entity.Transform.Pos.X - cam.Position.X) * dt * 5.0f,
                        cam.Position.Y + (entity.Transform.Pos.Y - cam.Position.Y) * dt * 5.0f);
            });

            cam.SetScreenSize(window.Size);
            renderer.BeginFrame(cam);
            world.Query(EntityFlag.Renderable, (entity) => {
                renderer.Draw(
                        entity.Transform,
                        entity.Color,
                        entity.Texture);

                if (entity == player) {
                    List<Entity> entities = world.SpatialQuery(entity.Transform.Pos, 5.0f);
                    foreach (Entity ent in entities) {
                        if (ent == player) {
                            continue;
                        }
                        renderer.Draw(
                                ent.Transform,
                                Color.RED);
                    }
                }
            });

            Vector2 worldPos = cam.ScreenToWorldSpace(Input.Instance.MousePosition);
            renderer.Draw(new Box() {
                    Pos = worldPos,
                    Size = new Vector2(1.0f),
                }, Color.GREEN);

            renderer.EndFrame();


            Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);
            renderer.BeginFrame(uiCam);
            Font font = AssetManager.Instance.GetFont("lato32");
            renderer.DrawText($"{player.Transform.Pos.X}, {player.Transform.Pos.Y}", font, new Vector2(8.0f), Color.WHITE);

            Vector2 screenPos = cam.WorldToScreenSpace(worldPos);
            renderer.Draw(new Box() {
                    Pos = screenPos,
                    Size = new Vector2(8.0f),
                }, Color.BLUE);
            renderer.EndFrame();
        }
    }
}
