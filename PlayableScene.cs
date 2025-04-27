using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    public abstract class PlayableScene {
        protected Window window;
        protected Renderer renderer;
        protected World world = new World();
        protected Player player;
        protected UI hud = new UI();
        protected bool updating = true;
        protected bool paused = false;

        public PlayableScene(Window window, Renderer renderer) {
            this.window = window;
            this.renderer = renderer;
            world.Camera.Zoom = 360.0f;
            player = world.SpawnEntity<Player>();
        }

        private void BuildUpgradeMenu() {
            Widget fullscreenContainer = hud.MakeWidget("##fullscreenContainer-aiuohweoih")
                .FixedSize(window.Size)
                .Floating(new Vector2())
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);

            Widget container = fullscreenContainer.MakeWidget("##cardContainer-aiuheifhuweif")
                .FlowHorizontal()
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center)
                .FitChildren();

            for (int i = 0; i < player.Upgrades.Length; i++) {
                Upgrade upgrade = player.Upgrades[i];
                upgrade.DrawHUD(container, i);
                if (upgrade.Selected) {
                    upgrade.Apply(player);
                    player.LeveledWithoutUpgrade = false;
                }

                if (i < player.Upgrades.Length - 1) {
                    container.MakeWidget($"##padding{i}-wefuiwehfoiuwehf")
                        .FixedWidth(32.0f);
                }
            }
        }

        private void BuildPlayerHUD(Player player) {
            Vector2 barSize = new Vector2(512, 32);
            // Health bar
            Widget container = hud.MakeWidget("player_hud_container")
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Right)
                .FitChildrenHeight()
                .FixedWidth(window.Size.X);
            Widget healthBar = container.MakeWidget("player_health_bar_bg")
                .Background(Color.HexRGB(0x241527))
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Right)
                .FixedSize(barSize);
            Vector2 percentLeft = barSize;
            percentLeft.X *= (float) player.Health / (float) player.MaxHealth;
            healthBar.MakeWidget($"{player.Health}/{player.MaxHealth} ##player_health_bar_fg")
                .Background(Color.HexRGB(0xcf573c))
                .FixedSize(percentLeft)
                .AlignText(WidgetTextAlignment.Right)
                .ShowText(AssetManager.Instance.GetFont("roboto_mono"), Color.WHITE);

            // XP bar
            Widget xpBar = container.MakeWidget("player_xp_bar_bg")
                .Background(Color.HexRGB(0x172038))
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Right)
                .FixedSize(barSize);
            percentLeft = barSize;
            percentLeft.X *= (float) player.Xp / (float) player.NeededXp;
            xpBar.MakeWidget($"{player.Xp}/{player.NeededXp} ##player_xp_bar_fg")
                .Background(Color.HexRGB(0x4f8fba))
                .FixedSize(percentLeft)
                .AlignText(WidgetTextAlignment.Right)
                .ShowText(AssetManager.Instance.GetFont("roboto_mono"), Color.WHITE);

            if (player.LeveledWithoutUpgrade) {
                BuildUpgradeMenu();
            }
        }

        private void BuildHealthBars() {
            Font font = AssetManager.Instance.GetFont("lato32");

            Widget bossBarContainer = hud.MakeWidget("boss_bar_container")
                .FixedSize(window.Size)
                .AlignChildren(WidgetAlignment.Bottom, WidgetAlignment.Center);

            // Health bars
            int id = 0;
            Vector2 enemyBarSize = new Vector2(32.0f, 4.0f);
            Vector2 screenPos = world.Camera.WorldToScreenSpace(player.Transform.Pos + new Vector2(0.0f, player.Transform.Size.Y / 2.0f));
            screenPos.Y -= 8.0f;
            screenPos.X -= enemyBarSize.X / 2.0f;

            Widget bar = hud.MakeWidget($"##bar{id}")
                .FixedSize(enemyBarSize)
                .Floating(screenPos)
                .Background(Color.HexRGB(0x241527));

            Vector2 healthLeft = enemyBarSize;
            healthLeft.X *= (float)player.Health / player.MaxHealth;
            bar.MakeWidget($"##bar_fg{id}")
                .FixedSize(healthLeft)
                .Floating(screenPos)
                .Background(Color.HexRGB(0xcf573c));

            world.OperateOnEntities((entity) => {
                if (entity is Enemy) {
                    Enemy enemy = (Enemy) entity;
                    Vector2 screenPos = world.Camera.WorldToScreenSpace(enemy.Transform.Pos + new Vector2(0.0f, enemy.Transform.Size.Y / 2.0f));
                    screenPos.Y -= 8.0f;
                    screenPos.X -= enemyBarSize.X / 2.0f;

                    Widget bar = hud.MakeWidget($"##bar{id}")
                        .FixedSize(enemyBarSize)
                        .Floating(screenPos)
                        .Background(Color.HexRGB(0x241527));

                    Vector2 healthLeft = enemyBarSize;
                    healthLeft.X *= (float) enemy.Health / enemy.MaxHealth;
                    bar.MakeWidget($"##bar_fg{id}")
                        .FixedSize(healthLeft)
                        .Floating(screenPos)
                        .Background(Color.HexRGB(0xcf573c));
                }

                // Boss bars
                if (entity is Boss) {
                    Boss boss = (Boss) entity;
                    font = AssetManager.Instance.GetFont("roboto_mono");
                    FontMetrics metrics = font.GetMetrics();

                    Vector2 padding = new Vector2(-64.0f, 4.0f);
                    Vector2 margin = new Vector2(64.0f, -16.0f);

                    Vector2 barSize = new Vector2(window.Size.X, metrics.Ascent - metrics.Descent);
                    barSize += padding * 2.0f;

                    Widget bar = bossBarContainer.MakeWidget($"awooga##boss_bar{id}")
                        .FixedSize(barSize)
                        .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center)
                        .RenderingExtension((widget, renderer) => {
                                Box box = widget.ComputedBox;
                                box.Size *= new Vector2((float) boss.Health / boss.MaxHealth, 1.0f);
                                renderer.Draw(box, Color.HexRGB(0xcf573c));
                            })
                        .Background(Color.HexRGB(0x241527));

                    bar.MakeWidget($"{boss.Health} / {boss.MaxHealth}##boss_health_text{id}")
                        .FitText()
                        .ShowText(font, Color.WHITE);

                    bossBarContainer.MakeWidget($"##boss_bar_padding{id}")
                        .FixedHeight(16.0f);
                }

                id++;
            });
        }

        private static bool Button(string text, Widget container) {
            Widget quitBtn = container.MakeWidget(text)
                .ShowText(AssetManager.Instance.GetFont("lato32"), Color.WHITE)
                .Background(Color.HexRGB(0x212121))
                .AlignText(WidgetTextAlignment.Center)
                .FixedSize(new Vector2(192, 64));
            if (quitBtn.Signal().Hovered) {
                quitBtn.Background(Color.HexRGB(0x303030));
                return Input.Instance.GetButtonOnDown(MouseButton.Left);
            }
            return false;
        }

        public virtual void Run(GameState gameState) {
            Debug.Instance.Camera = world.Camera;

            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            Color clearColor = Color.HexRGB(0x090a14);
            GL.ClearColor(clearColor.R, clearColor.G, clearColor.B, clearColor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (updating && !paused) {
                world.Update(gameState.DeltaTime);
            }

            world.Camera.ScreenSize = window.Size;
            renderer.BeginFrame(world.Camera);
            world.OperateOnEntities((entity) => {
                    if (!entity.Render) {
                        return;
                    }
                    renderer.Draw(
                        entity.Transform,
                        entity.Color,
                        entity.Texture);
                });
            renderer.EndFrame();

            Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);
            Font font = AssetManager.Instance.GetFont("lato32");

            hud.Begin(Input.Instance.MousePosition);

            BuildHealthBars();
            BuildPlayerHUD(player);

            if (player.Health <= 0) {
                player.Health = 0;
                updating = false;

                Widget container = hud.MakeWidget("game_over_container")
                    .FixedSize(window.Size)
                    .Floating(new Vector2())
                    .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);

                Widget panel = container.MakeWidget("game_over_panel")
                    .Background(Color.HexRGBA(0x394a50))
                    .FixedSize(new Vector2(256, 320))
                    .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);
                panel.MakeWidget("Game Over")
                    .FitText()
                    .ShowText(AssetManager.Instance.GetFont("lato48"), Color.HexRGB(0xcf573c));

                panel.MakeWidget("paddingohwieufhwieuf")
                    .FixedHeight(8);

                if (Button("Restart", panel)) {
                    gameState.InteractiveScene = new Game(window, renderer);
                }

                panel.MakeWidget("paddingowieuhrwieurhiwuer")
                    .FixedHeight(8);

                if (Button("Main Menu", panel)) {
                    gameState.Scene = Scene.MainMenu;
                }

                panel.MakeWidget("paddingiu234yukhsjbf")
                    .FixedHeight(8);

                if (Button("Quit", panel)) {
                    window.Close();
                }
            }

            if (paused) {
                font = AssetManager.Instance.GetFont("lato32");
                Widget container = hud.MakeWidget("paused_container")
                    .FixedSize(window.Size)
                    .Floating(new Vector2())
                    .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);

                Widget panel = container.MakeWidget("paused_panel")
                    .Background(Color.HexRGB(0x151d28))
                    .FixedSize(new Vector2(256, 320))
                    .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);
                panel.MakeWidget("Paused")
                    .FitText()
                    .ShowText(AssetManager.Instance.GetFont("lato48"), Color.HexRGB(0xde9e41));

                panel.MakeWidget("paddingohwieufhwieuf")
                    .FixedHeight(8);

                if (Button("Restart", panel)) {
                    gameState.InteractiveScene = new Game(window, renderer);
                }

                panel.MakeWidget("paddingowieuhrwieurhiwuer")
                    .FixedHeight(8);

                if (Button("Main Menu", panel)) {
                    gameState.Scene = Scene.MainMenu;
                }

                panel.MakeWidget("paddingiu234yukhsjbf")
                    .FixedHeight(8);

                if (Button("Quit", panel)) {
                    window.Close();
                }
            }

            if (Input.Instance.GetKeyOnDown(SDL.SDL_Keycode.SDLK_ESCAPE)) {
                paused = !paused;
            }

            hud.End();
            hud.Draw(renderer, window.Size);
        }
    }
}
