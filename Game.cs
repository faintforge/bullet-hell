using System.Diagnostics;

namespace BulletHell {
    public class Game : PlayableScene {
        private UI debugUI = new UI();
        private WaveSpawner spawner;

        public Game(Window window, Renderer renderer) : base(window, renderer) {
            spawner = world.SpawnEntity<WaveSpawner>();
            spawner.Player = player;
        }

        public override void Run(GameState gameState) {
            base.Run(gameState);

            hud.Begin(Input.Instance.MousePosition);
            Widget container = hud.MakeWidget("container")
                .FitChildrenHeight()
                .FixedWidth(window.Size.X)
                .AlignChildren(WidgetAlignment.Center, WidgetAlignment.Center);
            container.MakeWidget("padding")
                .FixedHeight(16.0f);
            container.MakeWidget($"Wave: {spawner.Wave}")
                .FitText()
                .ShowText(AssetManager.Instance.GetFont("lato24"), Color.WHITE);
            hud.End();
            hud.Draw(renderer, window.Size);

            if (player.Health > 0) {
                updating = !player.LeveledWithoutUpgrade;
            }
            BuildDebugUI();
            debugUI.Draw(renderer, window.Size);
            Profiler.Instance.Reset();
        }

        [Conditional("DEBUG")]
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
            Widget container = parent.MakeWidget($"##container{profile.GetHashCode()}{depth}")
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
    }
}
