namespace BulletHell {
    public class Tutorial : PlayableScene {
        private enum Stage {
            Pause,
            Unpause,
            Move,
            Shoot,
            Xp,
            Upgrade,
            Finished,
        }

        private Stage stage = Stage.Pause;
        private UI ui = new UI();
        private ParticleEmitter? goal;
        private Goblin? goblin;

        public Tutorial(Window window, Renderer renderer) : base(window, renderer) { }

        private void Text(string text, Widget parent) {
            parent.MakeWidget(text)
                .ShowText(AssetManager.Instance.GetFont("lato24"), Color.WHITE)
                // .Background(Color.HexRGB(0x394a50))
                .FitText();
        }

        public override void Run(GameState gameState) {
            base.Run(gameState);

            ui.Begin(Input.Instance.MousePosition);

            Widget container = ui.MakeWidget("container")
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center)
                .FixedSize(window.Size);

            switch (stage) {
                case Stage.Pause:
                    updating = false;
                    Text("To pause the game, press ESC.", container);
                    container.MakeWidget("aughaha")
                        .FixedHeight(256);
                    if (paused) {
                        stage = Stage.Unpause;
                    }
                    break;
                case Stage.Unpause:
                    Text("To unpause the game, press ESC again", container);
                    Text("or press the 'Resume' button.", container);
                    container.MakeWidget("aughaha")
                        .FixedHeight(512);
                    if (!paused) {
                        stage = Stage.Move;

                        goal = world.SpawnEntity<ParticleEmitter>();
                        goal.Transform.Pos = new Vector2(116.0f, 0.0f);
                        goal.Cfg = new ParticleEmitter.Config() {
                            Color = Color.HexRGB(0xa8ca58),
                            Time = 1.0f,
                            Count = 100,
                            Continuous = true,
                            Size = new Vector2(2.0f),
                            FinalOpacity = 0.0f,
                            FinalSize = 0.0f,
                            MinLifespan = 0.25f,
                            MaxLifespan = 1.0f,
                            RotationMin = 0.0f,
                            RotationMax = MathF.PI * 2.0f,
                            VelocitySpeedMax = 10.0f,
                            VelocitySpeedMin = 0.0f,
                            SpawnAngle = MathF.PI * 2.0f,
                            SpawnRadius = 8.0f,
                        };
                    }
                    break;
                case Stage.Move:
                    updating = true;
                    Text("Move to the green particles using WASD.", container);
                    container.MakeWidget("aughaha")
                        .FixedHeight(256);
                    if (player.Transform.Pos.Magnitude() >= 100.0f) {
                        stage = Stage.Shoot;
                        if (goal != null) {
                            goal.Kill();
                        }
                        goblin = world.SpawnEntity<Goblin>();
                    }
                    break;
                case Stage.Shoot:
                    Text("Aim with your cursor at the goblin.", container);
                    Text("Shoot with LMB.", container);
                    container.MakeWidget("aughaha")
                        .FixedHeight(256);
                    if (goblin != null && !goblin.Alive) {
                        stage = Stage.Xp;

                        float spawnRadius = 8.0f;
                        Random rng = new Random();
                        for (int i = 0; i < 50; i++) {
                            float angle = (float) rng.NextDouble() * 2.0f * MathF.PI;
                            float distance = spawnRadius * MathF.Sqrt((float) rng.NextDouble());
                            Vector2 pos = Vector2.FromAngle(angle) * distance;
                            XpPoint xp = world.SpawnEntity<XpPoint>();
                            xp.Transform.Pos = pos;
                        }
                    }
                    break;
                case Stage.Xp:
                    Text("Pick up the blue orbs to level up.", container);
                    container.MakeWidget("aughaha")
                        .FixedHeight(256);
                    if (player.LeveledWithoutUpgrade) {
                        stage = Stage.Upgrade;
                    }
                    break;
                case Stage.Upgrade:
                    Text("Click one of the upgrade cards to gain power.", container);
                    container.MakeWidget("aughaha")
                        .FixedHeight(512);
                    if (!player.LeveledWithoutUpgrade) {
                        stage = Stage.Finished;
                    }
                    break;
                case Stage.Finished:
                    if (!paused) {
                        Text("Tutorial completed!", container);
                        container.MakeWidget("aughahaauh")
                            .FixedHeight(32);
                        Text("Go back to the main menu", container);
                        Text("through the pause menu.", container);
                        container.MakeWidget("aughaha")
                            .FixedHeight(256);
                    }
                    break;
            }

            ui.End();
            ui.Draw(renderer, window.Size);
        }
    }
}
