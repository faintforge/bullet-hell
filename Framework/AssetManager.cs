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
        private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        private AssetManager() {}

        /// <summary>
        /// Loads a font from the system and assigns a name to it for easy access.
        /// </summary>
        /// <param name="name">Name of the asset.</param>
        /// <param name="filepath">Paht to font file.</param>
        /// <param name="size">Size of font.</param>
        public void LoadFont(string name, string filepath, int size) {
            Font font = new Font(filepath, size, new Vector2(512));
            fonts.Add(name, font);
        }

        /// <summary>
        /// Retrieve previously loaded font using its assigned name. 
        /// </summary>
        /// <param name="name">Name of the asset.</param>
        /// <returns>The requested font.</returns>
        /// <exception cref="Exception">Thorws an exceptions if no font with the requested name has been loaded.</exception>
        public Font GetFont(string name) {
            Font? font;
            if (!fonts.TryGetValue(name, out font) || font == null) {
                throw new Exception($"Font {name} not found!");
            }
            return font;
        }

        /// <summary>
        /// Loads a texture from the system and assigns a name to it for easy access.
        /// </summary>
        /// <param name="name">Name of the asset.</param>
        /// <param name="filepath">Paht to texture.</param>
        /// <param name="filter">Which filtering to use when scaling texture.</param>
        public void LoadTexture(string name, string filepath, TextureFilter filter) {
            Texture texture = Texture.FromFile(filepath, filter);
            textures.Add(name, texture);
        }

        /// <summary>
        /// Retrieve previously loaded texture using its assigned name. 
        /// </summary>
        /// <param name="name">Name of the asset.</param>
        /// <returns>The requested texture.</returns>
        /// <exception cref="Exception">Thorws an exceptions if no texture with the requested name has been loaded.</exception>
        public Texture GetTexture(string name) {
            Texture? texture;
            if (!textures.TryGetValue(name, out texture) || texture == null) {
                throw new Exception($"Texture {name} not found!");
            }
            return texture;
        }
    }
}
