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
        public Camera? Camera { get; set; }

        private Renderer renderer = new Renderer(64);

        private Debug() {}

        public void DrawBox(Box box, Color color) {
            if (Camera == null) {
                throw new Exception("Camera must be set before doing debug rendering.");
            }
            renderer.BeginFrame(Camera);
            renderer.Draw(box, color);
            renderer.EndFrame();
        }
    }
}
