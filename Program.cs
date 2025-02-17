using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    internal class Program {
        private enum Scene {
            MainMenu,
            Game,
        }

        static void Main(string[] args) {
            Window window = new Window(
                    "Window",
                    800, 600,
                    resizable: false,
                    vsync: true,
                    fullscreen: false
                );
            Renderer renderer = new Renderer();
            Scene scene = Scene.MainMenu;

            Game game = new Game(window, renderer);

            LoadAssets();

            uint last = SDL.SDL_GetTicks();
            float dt = 0.0f;
            while (window.Open) {
                uint curr = SDL.SDL_GetTicks();
                dt = (float) (curr - last) / 1000.0f;
                last = curr;

                switch (scene) {
                    case Scene.MainMenu:
                        MainMenu(window, renderer);
                        break;
                    case Scene.Game:
                        game.Run(dt);
                        break;
                }

                if (Input.Instance.GetKeyOnDown(SDL.SDL_Keycode.SDLK_F11)) {
                    window.ToggleFullscreen();
                }

                Input.Instance.ResetFrame();
                window.PollEvents();
                window.SwapBuffers();
            }
        }

        private static void LoadAssets() {
            AssetManager.Instance.LoadFont("lato48", "/usr/share/fonts/TTF/Lato-Regular.ttf", 48);
            AssetManager.Instance.LoadFont("lato32", "/usr/share/fonts/TTF/Lato-Regular.ttf", 32);
        }

        private static void MainMenu(Window window, Renderer renderer) {
            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Font titleFont = AssetManager.Instance.GetFont("lato48");
            Font font = AssetManager.Instance.GetFont("lato32");
            Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);

            renderer.BeginFrame(uiCam);

            Vector2 pos = new Vector2(64.0f);
            renderer.DrawText("Bullet Hell", titleFont, pos, Color.HSV(SDL.SDL_GetTicks() / 10.0f, 0.75f, 1.0f));
            pos.Y += titleFont.GetMetrics().LineGap;
            pos.Y += 16.0f;

            Box playBox = new Box() {
                Origin = new Vector2(-1.0f),
                Pos = pos,
                Size = font.MeasureText("Play") + 16.0f,
            };
            renderer.Draw(playBox, Color.HexRGB(0x212121));
            pos += 8.0f;
            renderer.DrawText("Play", font, pos, Color.WHITE);
            pos.X -= 8.0f;
            pos.Y += font.GetMetrics().LineGap;
            pos.Y += 16.0f;

            Box quitBox = new Box() {
                Origin = new Vector2(-1.0f),
                Pos = pos,
                Size = font.MeasureText("Quit") + 16.0f,
            };
            renderer.Draw(quitBox, Color.HexRGB(0x212121));
            pos += 8.0f;
            renderer.DrawText("Quit", font, pos, Color.WHITE);
            pos.X -= 8.0f;
            pos.Y += font.GetMetrics().LineGap;
            pos.Y += 16.0f;

            renderer.EndFrame();
        }
    }
}
