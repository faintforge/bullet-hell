namespace BulletHell {
    public enum Scene {
        MainMenu,
        Game,
    }

    public class GameState {
        public Scene Scene { get; set; } = Scene.MainMenu;
        public float DeltaTime { get; set; }
        public PlayableScene? InteractiveScene { get; set; }
    }
}
