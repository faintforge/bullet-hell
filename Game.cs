using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    public class Game {
        private Window window;
        private Renderer renderer;
        private World world = new World();
        private Boss boss;

        private bool paused = false;

        public Game(Window window, Renderer renderer) {
            this.window = window;
            this.renderer = renderer;

            world.Camera.Zoom = 360.0f;
            world.SpawnEntity<Player>();

            // world.SpawnEntity<GoblinSpawner>();
            boss = world.SpawnEntity<Boss>();
            boss.Transform.Pos = new Vector2();
        }

        public void Run(float deltaTime) {
            Debug.Instance.Camera = world.Camera;

            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            Color clearColor = Color.HexRGB(0x090a14);
            GL.ClearColor(clearColor.R, clearColor.G, clearColor.B, clearColor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (!paused) {
                world.Update(deltaTime);
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
            renderer.BeginFrame(uiCam);

            // Health bars
            world.OperateOnEntities((entity) => {
                    Vector2 barSize = new Vector2(32.0f, 4.0f);
                    if (entity is Player) {
                        Player player = (Player) entity;
                        Vector2 screenPos = world.Camera.WorldToScreenSpace(player.Transform.Pos + new Vector2(0.0f, player.Transform.Size.Y / 2.0f));
                        screenPos.Y -= 8.0f;
                        screenPos.X -= barSize.X / 2.0f;

                        // Background
                        renderer.Draw(new Box() {
                                Origin = new Vector2(-1.0f, 1.0f),
                                Pos = screenPos,
                                Size = barSize,
                            }, Color.HexRGB(0x241527));
                        // Foreground
                        Vector2 healthLeft = barSize;
                        healthLeft.X *= (float) player.Health / player.MaxHealth;
                        renderer.Draw(new Box() {
                                Origin = new Vector2(-1.0f, 1.0f),
                                Pos = screenPos,
                                Size = healthLeft,
                            }, Color.HexRGB(0xcf573c));
                    }

                    if (entity is Enemy) {
                        Enemy enemy = (Enemy) entity;
                        Vector2 screenPos = world.Camera.WorldToScreenSpace(enemy.Transform.Pos + new Vector2(0.0f, enemy.Transform.Size.Y / 2.0f));
                        screenPos.Y -= 8.0f;
                        screenPos.X -= barSize.X / 2.0f;

                        // Background
                        renderer.Draw(new Box() {
                                Origin = new Vector2(-1.0f, 1.0f),
                                Pos = screenPos,
                                Size = barSize,
                            }, Color.HexRGB(0x241527));
                        // Foreground
                        Vector2 healthLeft = barSize;
                        healthLeft.X *= (float) enemy.Health / enemy.MaxHealth;
                        renderer.Draw(new Box() {
                                Origin = new Vector2(-1.0f, 1.0f),
                                Pos = screenPos,
                                Size = healthLeft,
                            }, Color.HexRGB(0xcf573c));
                    }
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

                // Background
                renderer.Draw(new Box() {
                        Origin = new Vector2(-1.0f),
                        Pos = screenPos,
                        Size = barSize,
                    }, Color.HexRGB(0x241527));
                // Foreground
                Vector2 healthLeft = barSize;
                healthLeft.X *= (float) boss.Health / boss.MaxHealth;
                renderer.Draw(new Box() {
                        Origin = new Vector2(-1.0f),
                        Pos = screenPos,
                        Size = healthLeft,
                    }, Color.HexRGB(0xcf573c));

                screenPos.Y += padding.Y;
                screenPos.X += barSize.X / 2.0f;
                string hpText = $"{boss.Health} / {boss.MaxHealth}";
                screenPos.X -= font.MeasureText(hpText).X / 2.0f;
                renderer.DrawText(hpText, font, screenPos, Color.WHITE);
            }

            renderer.EndFrame();

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

            PrintProfiles();
            Profiler.Instance.Reset();
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
