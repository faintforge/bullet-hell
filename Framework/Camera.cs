namespace BulletHell {
    public class Camera {
        public bool InvertY { get; private set; }
        public Matrix4 Projection { get; private set; }
        public Matrix4 InverseProjection { get; private set; }
        public Vector2 Position { get; set; }

        private Vector2 screenSize;
        public Vector2 ScreenSize {
            get => screenSize;
            set {
                screenSize = value;
                CalculateProjectionMatrices();
            }
        }

        private float zoom;
        public float Zoom {
            get => zoom;
            set {
                zoom = value;
                CalculateProjectionMatrices();
            }
        }


        /// <summary>
        /// Create an instance of a Camera object.
        /// </summary>
        /// <param name="screenSize">Size of the screen.</param>
        /// <param name="position">Camera position.</param>
        /// <param name="zoom">How many units fit vertically on screen.</param>
        /// <param name="invertY">Is the y-axis inverted - positive Y is down instead of up.</param>
        public Camera(Vector2 screenSize, Vector2 position, float zoom, bool invertY = false) {
            ScreenSize = screenSize;
            Position = position;
            Zoom = zoom;
            InvertY = invertY;
            CalculateProjectionMatrices();
        }

        private void CalculateProjectionMatrices() {
            float halfZoom = Zoom / 2.0f;
            float aspect = (ScreenSize.X / ScreenSize.Y) * halfZoom;
            Projection = Matrix4.OrthographicProjection(-aspect, aspect, halfZoom, -halfZoom, -1.0f, 1.0f);
            InverseProjection = Matrix4.InverseOrthographicProjection(-aspect, aspect, halfZoom, -halfZoom, -1.0f, 1.0f);
        }

        /// <summary>
        /// Converts world space coordinates to screen space.
        /// </summary>
        /// <param name="world">World space coordinate.</param>
        /// <returns>Screen space coordinate.</returns>
        public Vector2 WorldToScreenSpace(Vector2 world) {
            world -= Position;
            Vector4 worldVec4 = new Vector4(world.X, world.Y, 0.0f, 1.0f);
            Vector4 normVec4 = Projection * worldVec4;
            Vector2 normVec2 = (new Vector2(normVec4.X, normVec4.Y) + 1.0f) / 2.0f;
            if (!InvertY) {
                normVec2.Y = 1.0f - normVec2.Y;
            }
            return normVec2 * ScreenSize;
        }

        /// <summary>
        /// Converts screen space coordinates to world space.
        /// </summary>
        /// <param name="screen">Screen space coordinate.</param>
        /// <returns>World space coordinate.</returns>
        public Vector2 ScreenToWorldSpace(Vector2 screen) {
            Vector2 norm = (screen / ScreenSize - 0.5f) * 2.0f;
            if (!InvertY) {
                norm.Y = -norm.Y;
            }
            Vector4 normVec4 = new Vector4(norm.X, norm.Y, 0.0f, 1.0f);
            Vector4 world = InverseProjection * normVec4;
            return new Vector2(world.X, world.Y) + Position;
        }
    }
}
