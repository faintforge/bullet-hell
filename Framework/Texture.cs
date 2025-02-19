using OpenTK.Graphics.OpenGL;
using SDL2;
using System.Runtime.InteropServices;

namespace BulletHell {
    public enum TextureFormat {
        RgbU8 = 3,
        RgbaU8 = 4,
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
            InternalFormat internalFormat = InternalFormat.Rgba8;;
            PixelFormat pixelFormat = PixelFormat.Rgba;
            PixelType pixelType = PixelType.UnsignedByte;
            switch (format) {
                case TextureFormat.RgbU8:
                    internalFormat = InternalFormat.Rgb8;
                    pixelFormat = PixelFormat.Rgb;
                    pixelType = PixelType.UnsignedByte;
                    break;
                case TextureFormat.RgbaU8:
                    internalFormat = InternalFormat.Rgba8;
                    pixelFormat = PixelFormat.Rgba;
                    pixelType = PixelType.UnsignedByte;
                    break;
            }

            Texture tex = new Texture();
            tex.handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, tex.handle);
            GL.TexImage2D<T>(TextureTarget.Texture2d, 0, internalFormat, (int) size.X, (int) size.Y, 0, pixelFormat, pixelType, data);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);

            tex.Size = size;

            return tex;
        }

        public static Texture FromFile(string filepath) {
            IntPtr surfacePtr = SDL_image.IMG_Load(filepath);
            if (surfacePtr == IntPtr.Zero) {
                throw new Exception($"Failed to load image file {filepath}!");
            }
            SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(surfacePtr);
            SDL.SDL_PixelFormat format = Marshal.PtrToStructure<SDL.SDL_PixelFormat>(surface.format);
            int width = surface.w;
            int height = surface.h;
            int channels = format.BytesPerPixel;

            byte[] surfacePixels = new byte[surface.pitch * height];
            Marshal.Copy(surface.pixels, surfacePixels, 0, surfacePixels.Length);

            byte[] textureData = new byte[width * height * channels];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    for (int channel = 0; channel < channels; channel++) {
                        textureData[(x + y * width) * channels + channel] = surfacePixels[(x * channels + y * surface.pitch) + channel];
                    }
                }
            }

            return Texture.Create(new Vector2(width, height), (TextureFormat) channels, textureData);
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
