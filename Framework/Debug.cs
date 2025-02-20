namespace BulletHell {
    public class Debug {
        private static Debug? instance;
        public static Debug Instance {
            get {
                if (instance == null) {
                    instance = new Debug();
                }
                return instance;
            }
        }

        private Renderer renderer = new Renderer(1);

        private Debug() {}

        public void DrawBox(Box box, Color color, Camera camera) {
            renderer.BeginFrame(camera);
            renderer.Draw(box, color);
            renderer.EndFrame();
        }
    }
}
