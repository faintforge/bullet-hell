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
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
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
                camPos.X = camPos.X + (pos.X - camPos.X) * dt * 5.0f;
                camPos.Y = camPos.Y + (pos.Y - camPos.Y) * dt * 5.0f;

                renderer.BeginFrame(window.Size, 50.0f, camPos);
                renderer.Draw(pos, new Vector2(1.0f), Color.HSV(SDL.SDL_GetTicks() / 10, 0.75f, 1.0f));
                renderer.Draw(new Vector2(), new Vector2(1.0f), Color.WHITE);
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
