using OpenTK.Graphics.OpenGL;

namespace BulletHell {
    public class Game {
        private Window window;
        private Renderer renderer;
        private World world = new World();

        public Game(Window window, Renderer renderer) {
            this.window = window;
            this.renderer = renderer;

            world.Camera.Zoom = 360.0f;
            world.SpawnEntity<Player>();

            world.SpawnEntity<GoblinSpawner>();

            // for (int y = 0; y < 32; y++) {
            //     for (int x = 0; x < 32; x++) {
            //         Goblin enemy = world.SpawnEntity<Goblin>();
            //         enemy.Transform.Pos = new Vector2(x, y) * 24;
            //     }
            // }
        }

        public void Run(float deltaTime) {
            Debug.Instance.Camera = world.Camera;

            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            world.Update(deltaTime);
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

            renderer.EndFrame();

            PrintProfiles();
            Profiler.Instance.Reset();
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

        private void PrintProfiles() {
            Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);
            Vector2 pos = new Vector2(8.0f);
            renderer.BeginFrame(uiCam);
            foreach (Profile prof in Profiler.Instance.Profiles.Values) {
                pos.Y = PrintProfilesHelper(prof, pos);
            }
            renderer.EndFrame();
        }
    }
}
