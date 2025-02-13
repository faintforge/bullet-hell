namespace BulletHell {
    public struct Color {
        public readonly static Color WHITE = Color.HexRGB(0xffffff);
        public readonly static Color BLACK = Color.HexRGB(0x000000);
        public readonly static Color RED = Color.HexRGB(0xff0000);
        public readonly static Color GREEN = Color.HexRGB(0x00ff00);
        public readonly static Color BLUE = Color.HexRGB(0x0000ff);

        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public static Color RGB(float r, float g, float b) {
            Color color = new Color();
            color.R = r;
            color.G = g;
            color.B = b;
            color.A = 1.0f;
            return color;
        }

        public static Color RGBA(float r, float g, float b, float a) {
            Color color = new Color();
            color.R = r;
            color.G = g;
            color.B = b;
            color.A = a;
            return color;
        }

        public static Color HexRGB(uint hex) {
            Color color = new Color();
            color.R = (float) ((hex >> (8 * 2)) & 0xff) / (float) 0xff;
            color.G = (float) ((hex >> (8 * 1)) & 0xff) / (float) 0xff;
            color.B = (float) ((hex >> (8 * 0)) & 0xff) / (float) 0xff;
            color.A = 1.0f;
            return color;
        }

        public static Color HexRGBA(uint hex) {
            Color color = new Color();
            color.R = (float) ((hex >> 8 * 3) & 0xff) / (float) 0xff;
            color.G = (float) ((hex >> 8 * 2) & 0xff) / (float) 0xff;
            color.B = (float) ((hex >> 8 * 1) & 0xff) / (float) 0xff;
            color.A = (float) ((hex >> 8 * 0) & 0xff) / (float) 0xff;
            return color;
        }

        public static Color HSL(float hue, float saturation, float lightness) {
            // https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB
            Color color = new Color();
            float chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            float hue_prime = Math.Abs(hue % 360.0f) / 60.0f;
            float x = chroma * (1.0f - Math.Abs((hue_prime % 2.0f) - 1.0f));
            if (hue_prime < 1.0f) { color = Color.RGB(chroma, x, 0.0f); }
            else if (hue_prime < 2.0f) { color = Color.RGB(x, chroma, 0.0f); }
            else if (hue_prime < 3.0f) { color = Color.RGB(0.0f, chroma, x); }
            else if (hue_prime < 4.0f) { color = Color.RGB(0.0f, x, chroma); }
            else if (hue_prime < 5.0f) { color = Color.RGB(x, 0.0f, chroma); }
            else if (hue_prime < 6.0f) { color = Color.RGB(chroma, 0.0f, x); }
            float m = lightness-chroma / 2.0f;
            color.R += m;
            color.G += m;
            color.B += m;
            return color;
        }

        public static Color HSV(float hue, float saturation, float value) {
            // https://en.wikipedia.org/wiki/HSL_and_HSV#HSV_to_RGB
            Color color = new Color();
            float chroma = value * saturation;
            float hue_prime = Math.Abs(hue % 360.0f) / 60.0f;
            float x = chroma * (1.0f - Math.Abs((hue_prime % 2.0f) - 1.0f));
            if (hue_prime < 1.0f) { color = Color.RGB(chroma, x, 0.0f); }
            else if (hue_prime < 2.0f) { color = Color.RGB(x, chroma, 0.0f); }
            else if (hue_prime < 3.0f) { color = Color.RGB(0.0f, chroma, x); }
            else if (hue_prime < 4.0f) { color = Color.RGB(0.0f, x, chroma); }
            else if (hue_prime < 5.0f) { color = Color.RGB(x, 0.0f, chroma); }
            else if (hue_prime < 6.0f) { color = Color.RGB(chroma, 0.0f, x); }
            float m = value - chroma;
            color.R += m;
            color.G += m;
            color.B += m;
            return color;
        }
    }
}
