namespace BulletHell {
    public class Utils {
        public static float Lerp(float a, float b, float t) {
            return a + (b - a) * t;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) {
            return a + (b - a) * t;
        }
    }
}
