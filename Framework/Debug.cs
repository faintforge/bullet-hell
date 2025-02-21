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

        public void DrawBoxOutline(Box box, Color color) {
            if (Camera == null) {
                throw new Exception("Camera must be set before doing debug rendering.");
            }
            Vector2[] vertices = box.GetVertices();
            renderer.BeginFrame(Camera);
            for (int i = 0; i < vertices.Length; i++) {
                DrawLine(vertices[(i + 1) % vertices.Length], vertices[i], Color.WHITE);
            }
            renderer.EndFrame();
        }

        public void DrawLine(Vector2 position, float length, float angle, Color color) {
            if (Camera == null) {
                throw new Exception("Camera must be set before doing debug rendering.");
            }
            renderer.BeginFrame(Camera);
            renderer.Draw(new Box() {
                    Pos = position,
                    Size = new Vector2(length, (0.5f / Camera.ScreenSize.Y) * Camera.Zoom),
                    Rot = angle,
                    Origin = new Vector2(-1.0f, 0.0f),
                }, color);
            renderer.EndFrame();
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color) {
            if (Camera == null) {
                throw new Exception("Camera must be set before doing debug rendering.");
            }
            Vector2 diff = end - start;
            float length = diff.Magnitude();
            float angle = MathF.Atan2(diff.Y, diff.X);
            renderer.BeginFrame(Camera);
            renderer.Draw(new Box() {
                    Pos = start,
                    Size = new Vector2(length, (0.5f / Camera.ScreenSize.Y) * Camera.Zoom),
                    Rot = angle,
                    Origin = new Vector2(-1.0f, 0.0f),
                }, color);
            renderer.EndFrame();
        }
    }
}
