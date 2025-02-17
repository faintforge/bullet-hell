namespace BulletHell {
    public class Camera {
        public Vector2 ScreenSize { get; private set; }
        public Vector2 Position { get; set; }
        public float Zoom { get; private set; }
        public bool InvertY { get; private set; }
        public Matrix4 Projection { get; private set; }

        public Camera(Vector2 screenSize, Vector2 position, float zoom, bool invertY = false) {
            ScreenSize = screenSize;
            Position = position;
            Zoom = zoom;
            InvertY = invertY;
            CalculateProjectionMatrix();
        }

        public void SetZoom(float zoom) {
            Zoom = zoom;
            CalculateProjectionMatrix();
        }

        public void SetScreenSize(Vector2 screenSize) {
            ScreenSize = screenSize;
            CalculateProjectionMatrix();
        }

        private void CalculateProjectionMatrix() {
            float halfZoom = Zoom / 2.0f;
            float aspect = (ScreenSize.X / ScreenSize.Y) * halfZoom;
            Projection = Matrix4.OrthographicProjection(-aspect, aspect, halfZoom, -halfZoom, -1.0f, 1.0f);
        }

        // public Vector2 WorldToScreenSpace(Vector2 world) {
        // }

        // public Vector2 ScreenToWorldSpace(Vector2 world) {
        // }
    }
}
