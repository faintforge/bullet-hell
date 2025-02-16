using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    internal class Program {
        static void Main(string[] args) {
            Window window = new Window(
                    "Window",
                    800, 600,
                    resizable: false,
                    vsync: true,
                    fullscreen: false
                );
            Renderer renderer = new Renderer();

            Font font = new Font("/usr/share/fonts/TTF/Lato-Regular.ttf", 32, new Vector2(512));

            Camera cam = new Camera(window.Size, new Vector2(), 50.0f);

            World world = new World();

            Entity player = world.SpawnEntity(EntityFlag.Player | EntityFlag.Renderable);
            player.Color = Color.RED;
            player.Name = "player";

            Entity spinner = world.SpawnEntity(EntityFlag.Renderable);
            spinner.Color = Color.GREEN;
            spinner.Name = "spinner";

            uint last = SDL.SDL_GetTicks();
            float dt = 0.0f;
            while (window.Open) {
                uint curr = SDL.SDL_GetTicks();
                dt = (float) (curr - last) / 1000.0f;
                last = curr;

                GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
                GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                spinner.Rot = (float) SDL.SDL_GetTicks() / 100.0f;

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
                    entity.Pos += vel * dt;

                    // Lerp camera to player position
                    cam.Position = new Vector2(
                            cam.Position.X + (entity.Pos.X - cam.Position.X) * dt * 5.0f,
                            cam.Position.Y + (entity.Pos.Y - cam.Position.Y) * dt * 5.0f);
                });

                cam.SetScreenSize(window.Size);
                renderer.BeginFrame(cam);
                world.Query(EntityFlag.Renderable, (entity) => {
                    renderer.Draw(
                            entity.Origin,
                            entity.Pos,
                            entity.Size,
                            entity.Color,
                            entity.Texture,
                            entity.Rot);
                });
                renderer.EndFrame();

                Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);
                renderer.BeginFrame(uiCam);
                string str = "The quick brown fox jumps over the lazy dog.";
                Vector2 gpos = new Vector2(8.0f);
                foreach (char c in str) {
                    Glyph g = font.GetGlyph(c);
                    renderer.DrawUV(new Vector2(-1.0f), gpos, g.Size, Color.WHITE, font.Atlas, g.UVs[0], g.UVs[1]);
                    gpos.X += g.Advance;
                }
                renderer.EndFrame();

                if (Input.Instance.GetKeyOnDown(SDL.SDL_Keycode.SDLK_F11)) {
                    window.ToggleFullscreen();
                }

                Input.Instance.ResetFrame();
                window.PollEvents();
                window.SwapBuffers();
            }
        }
    }
}
