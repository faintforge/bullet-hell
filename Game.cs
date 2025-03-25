using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    public class Game {
        private Window window;
        private Renderer renderer;
        private World world = new World();
        private Boss? boss;

        private bool paused = false;
        private UI debugUI = new UI();
        private UI hud = new UI();

        public Game(Window window, Renderer renderer) {
            this.window = window;
            this.renderer = renderer;

            world.Camera.Zoom = 360.0f;
            Player player = world.SpawnEntity<Player>();

            world.SpawnEntity<GoblinSpawner>();

            // boss = world.SpawnEntity<Boss>();
            // boss.Transform.Pos = new Vector2();
        }

        public void Run(float deltaTime) {
            Debug.Instance.Camera = world.Camera;

            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            Color clearColor = Color.HexRGB(0x090a14);
            GL.ClearColor(clearColor.R, clearColor.G, clearColor.B, clearColor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (!paused) {
                if (Input.Instance.GetKeyOnDown(SDL.SDL_Keycode.SDLK_SPACE)) {
                    Vector2 pos = world.Camera.ScreenToWorldSpace(Input.Instance.MousePosition);
                    Goblin goblin = world.SpawnEntity<Goblin>();
                    goblin.Transform.Pos = pos;
                }

                world.Update(deltaTime);
            }

            world.Camera.ScreenSize = window.Size;
            renderer.BeginFrame(world.Camera);

            // Vector2 mousePos = world.Camera.ScreenToWorldSpace(Input.Instance.MousePosition);
            // renderer.Draw(new Box() {
            //         Pos = mousePos,
            //         Size = new Vector2(128.0f),
            //     }, Color.RED);
            //
            // Box obj = new Box() {
            //     Pos = new Vector2(128.0f),
            //     Size = new Vector2(16.0f),
            // };
            // if (obj.GetBoundingAABB().IntersectsCircle(mousePos, 64.0f)) {
            //     renderer.Draw(obj, Color.BLUE);
            // } else {
            //     renderer.Draw(obj, Color.GREEN);
            // }

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
            // renderer.BeginFrame(uiCam);

            hud.Begin(Input.Instance.MousePosition);
            // Health bars
            int id = 0;
            world.OperateOnEntities((entity) => {
                Vector2 barSize = new Vector2(32.0f, 4.0f);
                if (entity is Player) {
                    Player player = (Player) entity;
                    Vector2 screenPos = world.Camera.WorldToScreenSpace(player.Transform.Pos + new Vector2(0.0f, player.Transform.Size.Y / 2.0f));
                    screenPos.Y -= 8.0f;
                    screenPos.X -= barSize.X / 2.0f;

                    Widget bar = hud.MakeWidget($"##bar{id}")
                        .FixedSize(barSize)
                        .Floating(screenPos)
                        .Background(Color.HexRGB(0x241527));

                    Vector2 healthLeft = barSize;
                    healthLeft.X *= (float) player.Health / player.MaxHealth;
                    bar.MakeWidget($"##bar_fg{id}")
                        .FixedSize(healthLeft)
                        .Floating(screenPos)
                        .Background(Color.HexRGB(0xcf573c));

                    BuildPlayerHUD(player);
                }

                if (entity is Enemy) {
                    Enemy enemy = (Enemy) entity;
                    Vector2 screenPos = world.Camera.WorldToScreenSpace(enemy.Transform.Pos + new Vector2(0.0f, enemy.Transform.Size.Y / 2.0f));
                    screenPos.Y -= 8.0f;
                    screenPos.X -= barSize.X / 2.0f;

                    Widget bar = hud.MakeWidget($"##bar{id}")
                        .FixedSize(barSize)
                        .Floating(screenPos)
                        .Background(Color.HexRGB(0x241527));

                    Vector2 healthLeft = barSize;
                    healthLeft.X *= (float) enemy.Health / enemy.MaxHealth;
                    bar.MakeWidget($"##bar_fg{id}")
                        .FixedSize(healthLeft)
                        .Floating(screenPos)
                        .Background(Color.HexRGB(0xcf573c));
                }
                id++;
            });

            // Boss bar
            if (boss != null) {
                font = AssetManager.Instance.GetFont("roboto_mono");
                FontMetrics metrics = font.GetMetrics();

                Vector2 padding = new Vector2(-64.0f, 4.0f);
                Vector2 margin = new Vector2(64.0f, -16.0f);

                Vector2 barSize = new Vector2(window.Size.X, metrics.Ascent - metrics.Descent);
                barSize += padding * 2.0f;

                Vector2 screenPos = new Vector2(0.0f, window.Size.Y - barSize.Y);
                screenPos += margin;

                Widget bar = hud.MakeWidget("##boss_bar")
                    .Floating(screenPos)
                    .FixedSize(barSize)
                    .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center)
                    .Background(Color.HexRGB(0x241527));

                Vector2 healthLeft = barSize;
                healthLeft.X *= (float) boss.Health / boss.MaxHealth;
                bar.MakeWidget("##boss_bar_fg")
                    .FixedSize(healthLeft)
                    .Floating(screenPos)
                    .Background(Color.HexRGB(0xcf573c));

                bar.MakeWidget($"{boss.Health} / {boss.MaxHealth}")
                    .FitText()
                    .ShowText(font, Color.WHITE);
            }

            // renderer.EndFrame();

            if (paused) {
                renderer.BeginFrame(uiCam);
                font = AssetManager.Instance.GetFont("lato32");
                string text = "Paused";
                Vector2 textSize = font.MeasureText(text);
                Vector2 pos = window.Size / 2.0f - textSize / 2.0f;
                pos.Y = 32.0f;
                renderer.DrawText(text, font, pos, Color.HexRGB(0xe8c170));
                renderer.EndFrame();
            }

            if (Input.Instance.GetKeyOnDown(SDL.SDL_Keycode.SDLK_ESCAPE)) {
                paused = !paused;
            }

            hud.End();
            hud.Draw(renderer, window.Size);

            BuildDebugUI();
            debugUI.Draw(renderer, window.Size);
            //PrintProfiles();
            Profiler.Instance.Reset();
        }

        private void BuildPlayerHUD(Player player) {
            // Health bar
            Widget container = hud.MakeWidget("player_hud_container")
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Right)
                .FitChildrenHeight()
                .FixedWidth(window.Size.X);
            Widget healthBar = container.MakeWidget("player_health_bar_bg")
                .Background(Color.HexRGB(0x241527))
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Right)
                .FixedSize(new Vector2(512, 32));
            float percentLeft = (float) player.Health / (float) player.MaxHealth;
            healthBar.MakeWidget($"{player.Health}/{player.MaxHealth} ##player_health_bar_fg")
                .Background(Color.HexRGB(0xcf573c))
                .FixedSize(new Vector2(512 * percentLeft, 32))
                .AlignText(WidgetTextAlignment.Right)
                .ShowText(AssetManager.Instance.GetFont("roboto_mono"), Color.WHITE);

            // XP bar
            Widget xpBar = container.MakeWidget("player_xp_bar_bg")
                .Background(Color.HexRGB(0x172038))
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Right)
                .FixedSize(new Vector2(512, 32));
            percentLeft = (float) player.Xp / (float) player.NeededXp;
            xpBar.MakeWidget($"{player.Xp}/{player.NeededXp} ##player_xp_bar_fg")
                .Background(Color.HexRGB(0x4f8fba))
                .FixedSize(new Vector2(512 * percentLeft, 32))
                .AlignText(WidgetTextAlignment.Right)
                .ShowText(AssetManager.Instance.GetFont("roboto_mono"), Color.WHITE);
        }

        private void BuildDebugUI() {
            debugUI.Begin(Input.Instance.MousePosition);
            Widget root = debugUI.MakeWidget("root");
            foreach (Profile profile in Profiler.Instance.Profiles.Values) {
                BuildDebugUIHelper(profile, root, 0);
            }
            Font font = AssetManager.Instance.GetFont("roboto_mono");
            root.MakeWidget($"Entities alive: {world.Entities.Count}")
                .ShowText(font, Color.WHITE)
                .FitText();
            debugUI.End();
        }

        private void BuildDebugUIHelper(Profile profile, Widget parent, int depth) {
            Font font = AssetManager.Instance.GetFont("roboto_mono");
            string text = $"{profile.TotalDuration:0.00}ms {profile.AverageDuration:0.00}ms {profile.CallCount} call(s) - {profile.Name}##{depth}";
            Widget container = parent.MakeWidget($"##container{profile.GetHashCode()}")
                .FitChildren()
                .FlowHorizontal();
            container.MakeWidget($"##padding{depth}{profile.GetHashCode()}")
                .FixedSize(new Vector2(depth * 25.0f, 0.0f));
            container.MakeWidget(text)
                .ShowText(font, Color.WHITE)
                .FitText();

            foreach (Profile prof in profile.ChildProfiles.Values) {
                BuildDebugUIHelper(prof, parent, depth + 1);
            }
        }

        private void PrintProfiles() {
            Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);
            Vector2 pos = new Vector2(8.0f);
            renderer.BeginFrame(uiCam);
            foreach (Profile prof in Profiler.Instance.Profiles.Values) {
                pos.Y = PrintProfilesHelper(prof, pos);
            }

            Font font = AssetManager.Instance.GetFont("roboto_mono");
            renderer.DrawText($"Entities alive: {world.Entities.Count}", font, pos, Color.WHITE);
            renderer.EndFrame();
        }

        private float PrintProfilesHelper(Profile profile, Vector2 pos) {
            Font font = AssetManager.Instance.GetFont("roboto_mono");
            FontMetrics metrics = font.GetMetrics();

            string text = $"{profile.TotalDuration:0.00}ms {profile.AverageDuration:0.00}ms {profile.CallCount} call(s) - {profile.Name}";
            renderer.DrawText(text, font, pos, Color.WHITE);

            pos.Y += metrics.LineGap;
            pos.X += 16.0f;
            foreach (Profile prof in profile.ChildProfiles.Values) {
                pos.Y = PrintProfilesHelper(prof, pos);
            }
            return pos.Y;
        }
    }
}
