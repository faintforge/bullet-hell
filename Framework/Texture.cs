using OpenTK.Graphics.OpenGL;

namespace BulletHell {
    public enum TextureFormat {
        RU8,
        RgbU8,
        RgbaU8,
    }

    public class Texture {
        private int handle;
        public Vector2 Size { get; private set; }

        private Texture() { }

        ~Texture() {
            GL.DeleteTexture(handle);
        }

        /// <summary>
        /// Create a texture.
        /// </summary>
        /// <typeparam name="T">Type of data used as pixel information.</typeparam>
        /// <param name="size">2D size of the texture.</param>
        /// <param name="format">Amount of channels and datatype used.</param>
        /// <param name="data">Pixel data.</param>
        /// <returns>Texture.</returns>
        public static Texture Create<T>(Vector2 size, TextureFormat format, T[] data)
            where T : unmanaged
        {
            Texture tex = new Texture();
            tex.handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, tex.handle);
            GL.TexImage2D<T>(TextureTarget.Texture2d, 0, InternalFormat.Rgba8, (int) size.X, (int) size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);

            tex.Size = size;

            return tex;
        }

        /// <summary>
        /// Bind texture to a particular slot for rendering.
        /// </summary>
        /// <param name="slot">Slot to bind to.</param>
        public void Bind(uint slot) {
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
            GL.BindTexture(TextureTarget.Texture2d, handle);
        }
    }
}
