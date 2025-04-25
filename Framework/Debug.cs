namespace BulletHell {
    public class Debug {
        private static Debug? instance;
        /// <summary>
        /// Singleton instance of this class.
        /// </summary>
        public static Debug Instance {
            get {
                if (instance == null) {
                    instance = new Debug();
                }
                return instance;
            }
        }
        /// <summary>
        /// World camera to project debug shapes with.
        /// </summary>
        public Camera? Camera { get; set; }

        private Renderer renderer = new Renderer(64);

        private Debug() {}

        /// <summary>
        /// Instantly a fully filled box.
        /// </summary>
        /// <param name="box">Box to draw.</param>
        /// <param name="color">Color of the box.</param>
        /// <exception cref="Exception">If the camera is not set before being called.</exception>
        public void DrawBox(Box box, Color color) {
            if (Camera == null) {
                throw new Exception("Camera must be set before doing debug rendering.");
            }
            renderer.BeginFrame(Camera);
            renderer.Draw(box, color);
            renderer.EndFrame();
        }

        /// <summary>
        /// Instantly draw the outline of a box.
        /// </summary>
        /// <param name="box">Box to outline.</param>
        /// <param name="color">Color of the box.</param>
        /// <exception cref="Exception">If the camera is not set before being called.</exception>
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

        /// <summary>
        /// Instantly draw a line with a length.
        /// </summary>
        /// <param name="position">Position of the start of the line.</param>
        /// <param name="length">Length of the line.</param>
        /// <param name="angle">Radians between the line and the X-axis.</param>
        /// <param name="color">Color of the line.</param>
        /// <exception cref="Exception">If the camera is not set before being called.</exception>
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

        /// <summary>
        /// Instantly draw a line between two positions.
        /// </summary>
        /// <param name="start">Start position of the line.</param>
        /// <param name="end">End positoin of the line.</param>
        /// <param name="color">Color of the line.</param>
        /// <exception cref="Exception">If the camera is not set before being called.</exception>
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
