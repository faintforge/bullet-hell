namespace BulletHell {
    public class Utils {
        /// <summary>
        /// Linearly interpolate between two floats.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Time component between 0 (a) and 1 (b).</param>
        /// <returns>Linearly interpolated value between a and b based on t.</returns>
        public static float Lerp(float a, float b, float t) {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Linearly interpolate between two vector2s.
        /// </summary>
        /// <param name="a">Starting value.</param>
        /// <param name="b">Ending value.</param>
        /// <param name="t">Time component between 0 (a) and 1 (b).</param>
        /// <returns>Linearly interpolated value between a and b based on t.</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) {
            return a + (b - a) * t;
        }
    }
}
