namespace BulletHell {
    public class AssetManager {
        private static AssetManager? instance;
        public static AssetManager Instance {
            get {
                if (instance == null) {
                    instance = new AssetManager();
                }
                return instance;
            }
        }

        private Dictionary<string, Font> fonts = new Dictionary<string, Font>();

        public void LoadFont(string name, string filepath, int size) {
            Font font = new Font(filepath, size, new Vector2(512));
            fonts.Add(name, font);
        }

        public Font GetFont(string name) {
            Font? font;
            if (!fonts.TryGetValue(name, out font) || font == null) {
                throw new Exception($"Font {name} not found!");
            }
            return font;
        }
    }
}
