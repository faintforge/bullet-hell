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

        public void Bind(uint slot) {
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
            GL.BindTexture(TextureTarget.Texture2d, handle);
        }
    }
}
