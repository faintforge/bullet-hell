using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    internal class Program {
        private enum Scene {
            MainMenu,
            Game,
        }

        private static void PrintProfilesHelper(Profile profile, int depth) {
            string spaces = "";
            for (int i = 0; i < depth * 4; i++) {
                spaces += ' ';
            }
            Console.WriteLine($"{spaces}{profile.TotalDuration:0.00}ms {profile.AverageDuration:0.00}ms {profile.CallCount} call(s) - {profile.Name}");

            foreach (Profile prof in profile.ChildProfiles.Values) {
                PrintProfilesHelper(prof, depth + 1);
            }
        }

        private static void PrintProfiles() {
            foreach (Profile prof in Profiler.Instance.Profiles.Values) {
                PrintProfilesHelper(prof, 0);
            }
        }

        private static void Main(string[] args) {
            Window window = new Window(
                    "Window",
                    800, 600,
                    resizable: false,
                    vsync: true,
                    fullscreen: true
                );
            Renderer renderer = new Renderer();
            // Scene scene = Scene.MainMenu;
            Scene scene = Scene.Game;

            LoadAssets();

            Game game = new Game(window, renderer);

            int fps = 0;
            uint lastFps = SDL.SDL_GetTicks();

            uint last = SDL.SDL_GetTicks();
            float dt = 0.0f;
            while (window.Open) {
                uint curr = SDL.SDL_GetTicks();
                dt = (float) (curr - last) / 1000.0f;
                last = curr;

                fps++;
                if (SDL.SDL_GetTicks() - lastFps >= 1000) {
                    Console.WriteLine($"FPS: {fps}");
                    fps = 0;
                    lastFps = SDL.SDL_GetTicks();
                }

                switch (scene) {
                    case Scene.MainMenu:
                        MainMenu(window, renderer, ref scene);
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
            AssetManager.Instance.LoadFont("lato48", "assets/fonts/Lato-Regular.ttf", 48);
            AssetManager.Instance.LoadFont("lato32", "assets/fonts/Lato-Regular.ttf", 32);
            AssetManager.Instance.LoadFont("roboto_mono", "assets/fonts/RobotoMono-Regular.ttf", 16);

            AssetManager.Instance.LoadTexture("player", "assets/textures/player.png");
            AssetManager.Instance.LoadTexture("firebolt", "assets/textures/firebolt.png");
            AssetManager.Instance.LoadTexture("enemy_dagger", "assets/textures/enemy_dagger.png");
            AssetManager.Instance.LoadTexture("goblin", "assets/textures/goblin.png");
            AssetManager.Instance.LoadTexture("boss", "assets/textures/boss.png");
            AssetManager.Instance.LoadTexture("crystal_shard", "assets/textures/crystal_shard.png");
        }

        private static Vector2 quitPos = new Vector2();
        private static void MainMenu(Window window, Renderer renderer, ref Scene scene) {
            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Font titleFont = AssetManager.Instance.GetFont("lato48");
            Font font = AssetManager.Instance.GetFont("lato32");
            Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);
            Color color;

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
            if (playBox.IntersectsPoint(Input.Instance.MousePosition)) {
                if (Input.Instance.GetButtonOnDown(MouseButton.Left)) {
                    scene = Scene.Game;
                }
                color = Color.HexRGB(0x303030);
            } else {
                color = Color.HexRGB(0x212121);
            }
            renderer.Draw(playBox, color);
            pos += 8.0f;
            renderer.DrawText("Play", font, pos, Color.WHITE);
            pos.X -= 8.0f;
            pos.Y += font.GetMetrics().LineGap;
            pos.Y += 16.0f;

            if (quitPos.X == 0.0f) {
                quitPos = pos;
            }
            Box quitBox = new Box() {
                Origin = new Vector2(-1.0f),
                Pos = quitPos,
                Size = font.MeasureText("Quit") + 16.0f,
            };
            if (quitBox.IntersectsPoint(Input.Instance.MousePosition)) {
                if (Input.Instance.GetButtonOnDown(MouseButton.Left)) {
                    Random rng = new Random();
                    quitPos = new Vector2((float) rng.NextDouble(), (float) rng.NextDouble()) * (window.Size - quitBox.Size);
                    // window.Close();
                }
                color = Color.HexRGB(0x303030);
            } else {
                color = Color.HexRGB(0x212121);
            }
            renderer.Draw(quitBox, color);
            quitPos += 8.0f;
            renderer.DrawText("Quit", font, quitPos, Color.WHITE);
            quitPos -= 8.0f;

            pos.Y += 8.0f;
            pos.Y += font.GetMetrics().LineGap;
            pos.Y += 16.0f;

            renderer.EndFrame();
        }
    }
}
