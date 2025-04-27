using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    internal class Program {
        private static void Main(string[] args) {
            Window window = new Window(
                    "Window",
                    800, 600,
                    resizable: false,
                    vsync: true,
                    fullscreen: true
                );
            Renderer renderer = new Renderer();
            UI mainMenuUI = new UI();
            GameState gameState = new GameState();

            LoadAssets();

            Game game = new Game(window, renderer);
            Tutorial tutorial = new Tutorial(window, renderer);

            int fps = 0;
            uint lastFps = SDL.SDL_GetTicks();

            uint last = SDL.SDL_GetTicks();
            while (window.Open) {
                uint curr = SDL.SDL_GetTicks();
                gameState.DeltaTime = (curr - last) / 1000.0f;
                last = curr;

                fps++;
                if (SDL.SDL_GetTicks() - lastFps >= 1000) {
                    Console.WriteLine($"FPS: {fps}");
                    fps = 0;
                    lastFps = SDL.SDL_GetTicks();
                }

                switch (gameState.Scene) {
                    case Scene.MainMenu:
                        MainMenu(window, renderer, gameState, mainMenuUI);
                        break;
                    case Scene.Game:
                        if (gameState.InteractiveScene != null) {
                            gameState.InteractiveScene.Run(gameState);
                        }
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

        private static bool Button(string text, Widget container) {
            Widget quitBtn = container.MakeWidget(text)
                .ShowText(AssetManager.Instance.GetFont("lato32"), Color.WHITE)
                .Background(Color.HexRGB(0x212121))
                .AlignText(WidgetTextAlignment.Center)
                .FixedSize(new Vector2(128, 64));
            if (quitBtn.Signal().Hovered) {
                quitBtn.Background(Color.HexRGB(0x303030));
                return Input.Instance.GetButtonOnDown(MouseButton.Left);
            }
            return false;
        }

        private static void MainMenu(Window window, Renderer renderer, GameState gameState, UI ui) {
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
            if (Button("Play", panel)) {
                gameState.Scene = Scene.Game;
                gameState.InteractiveScene = new Game(window, renderer);
            }
            panel.MakeWidget("padding3")
                .FixedHeight(8);
            if (Button("Tutorial", panel)) {
                gameState.Scene = Scene.Game;
                gameState.InteractiveScene = new Tutorial(window, renderer);
            }
            panel.MakeWidget("padding2")
                .FixedHeight(8);
            if (Button("Quit", panel)) {
                window.Close();
            }

            ui.End();
            ui.Draw(renderer, window.Size);
        }
    }
}
