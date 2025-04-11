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
                    fullscreen: false
                );
            Renderer renderer = new Renderer();
            UI mainMenuUI = new UI();
            // Scene scene = Scene.MainMenu;
            Scene scene = Scene.Game;

            LoadAssets();

            Game game = new Game(window, renderer);

            int fps = 0;
            uint lastFps = SDL.SDL_GetTicks();

            uint last = SDL.SDL_GetTicks();
            float dt;
            while (window.Open) {
                uint curr = SDL.SDL_GetTicks();
                dt = (curr - last) / 1000.0f;
                last = curr;

                fps++;
                if (SDL.SDL_GetTicks() - lastFps >= 1000) {
                    Console.WriteLine($"FPS: {fps}");
                    fps = 0;
                    lastFps = SDL.SDL_GetTicks();
                }

                switch (scene) {
                    case Scene.MainMenu:
                        MainMenu(window, renderer, ref scene, mainMenuUI);
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
            AssetManager.Instance.LoadFont("lato24", "assets/fonts/Lato-Regular.ttf", 24);
            AssetManager.Instance.LoadFont("roboto_mono", "assets/fonts/RobotoMono-Regular.ttf", 16);

            AssetManager.Instance.LoadTexture("player", "assets/textures/player.png", TextureFilter.Nearest);
            AssetManager.Instance.LoadTexture("firebolt", "assets/textures/firebolt.png", TextureFilter.Nearest);
            AssetManager.Instance.LoadTexture("enemy_dagger", "assets/textures/enemy_dagger.png", TextureFilter.Nearest);
            AssetManager.Instance.LoadTexture("goblin", "assets/textures/goblin.png", TextureFilter.Nearest);
            AssetManager.Instance.LoadTexture("boss", "assets/textures/boss.png", TextureFilter.Nearest);
            AssetManager.Instance.LoadTexture("crystal_shard", "assets/textures/crystal_shard.png", TextureFilter.Nearest);
            AssetManager.Instance.LoadTexture("beam_projectile", "assets/textures/beam_projectile.png", TextureFilter.Nearest);
            AssetManager.Instance.LoadTexture("rat", "assets/textures/rat.png", TextureFilter.Nearest);
        }

        private static void MainMenu(Window window, Renderer renderer, ref Scene scene, UI ui) {
            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            ui.Begin(Input.Instance.MousePosition);

            Widget container = ui.MakeWidget("container")
                .FixedSize(window.Size)
                .Background(Color.BLACK)
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);

            Widget panel = container.MakeWidget("panel")
                .FitChildren()
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);

            panel.MakeWidget("Bullet Hell")
                .ShowText(AssetManager.Instance.GetFont("lato48"), Color.HSV(SDL.SDL_GetTicks() / 10.0f, 0.75f, 1.0f))
                .FitText();
            panel.MakeWidget("padding1")
                .FixedHeight(8);

            Widget playBtn = panel.MakeWidget("Play")
                .ShowText(AssetManager.Instance.GetFont("lato32"), Color.WHITE)
                .Background(Color.HexRGB(0x212121))
                .AlignText(WidgetTextAlignment.Center)
                .FixedSize(new Vector2(96, 64));
            panel.MakeWidget("padding2")
                .FixedHeight(8);
            if (playBtn.Signal().Hovered) {
                playBtn.Background(Color.HexRGB(0x303030));
                if (Input.Instance.GetButtonOnDown(MouseButton.Left)) {
                    scene = Scene.Game;
                }
            }

            Widget quitBtn = panel.MakeWidget("Quit")
                .ShowText(AssetManager.Instance.GetFont("lato32"), Color.WHITE)
                .Background(Color.HexRGB(0x212121))
                .AlignText(WidgetTextAlignment.Center)
                .FixedSize(new Vector2(96, 64));
            if (quitBtn.Signal().Hovered) {
                quitBtn.Background(Color.HexRGB(0x303030));
                if (Input.Instance.GetButtonOnDown(MouseButton.Left)) {
                }
            }

            ui.End();
            ui.Draw(renderer, window.Size);
        }
    }
}
