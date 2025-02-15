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
                    fullscreen: true
                );
            Renderer renderer = new Renderer();

            Font font = new Font("/usr/share/fonts/TTF/SplineSans-Regular.ttf", 32, new Vector2(512));

            Vector2 vec = new Vector2(1.0f, 0.0f);
            vec.Rotate(45.0f);

            const float speed = 25.0f;
            Vector2 pos = new Vector2();
            Vector2 camPos = new Vector2();

            uint last = SDL.SDL_GetTicks();
            float dt = 0.0f;
            while (window.Open) {
                uint curr = SDL.SDL_GetTicks();
                dt = (float) (curr - last) / 1000.0f;
                last = curr;

                GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
                GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                Vector2 vel = new Vector2();
                vel.X -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_a));
                vel.X += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_d));
                vel.Y -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_s));
                vel.Y += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_w));
                if (vel.MagnitudeSquared() != 0.0f) {
                    vel.Normalize();
                }
                vel *= speed;

                pos += vel * dt;
                // Lerp camera to player position
                camPos.X = camPos.X + (pos.X - camPos.X) * dt * 5.0f;
                camPos.Y = camPos.Y + (pos.Y - camPos.Y) * dt * 5.0f;

                renderer.BeginFrame(window.Size, 50.0f, camPos);
                renderer.Draw(pos, new Vector2(1.0f), Color.HSV(SDL.SDL_GetTicks() / 10, 0.75f, 1.0f));
                renderer.Draw(new Vector2(), new Vector2(1.0f), Color.WHITE, null, (float) SDL.SDL_GetTicks() / 1000.0f);
                renderer.EndFrame();

                renderer.BeginFrame(window.Size, window.Size.Y, new Vector2());
                renderer.Draw(new Vector2(), font.Atlas.Size, Color.WHITE, font.Atlas);
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
