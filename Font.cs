using SDL2;
using System.Runtime.InteropServices;

namespace BulletHell {
    public class Font {
        public Texture Atlas { get; private set; }
        IntPtr sdlFont;

        public Font(string filepath, int size, Vector2 atlasSize) {
            sdlFont = SDL_ttf.TTF_OpenFont(filepath, size);
            if (sdlFont == IntPtr.Zero) {
                Console.WriteLine($"Failed to load font {filepath}!");
                Environment.Exit(1);
            }

            byte[] atlasData = new byte[(int) (atlasSize.X * atlasSize.Y) * 4];

            Vector2 atlasPos = new Vector2();
            float rowHeight = 0;
            const byte ASCII_START = 32;
            const byte ASCII_END = 126;
            for (ushort c = ASCII_START; c <= ASCII_END; c++) {
                IntPtr glyphPtr = SDL_ttf.TTF_RenderGlyph_Blended(sdlFont, c, new SDL.SDL_Color{r=255, g=255, b=255, a=255});
                SDL.SDL_Surface glyphSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(glyphPtr);

                byte[] surfacePixels = new byte[glyphSurface.pitch * glyphSurface.h];
                Marshal.Copy(glyphSurface.pixels, surfacePixels, 0, glyphSurface.h * glyphSurface.pitch);

                if (atlasPos.X + glyphSurface.w > atlasSize.X) {
                    atlasPos.X = 0;
                    atlasPos.Y += rowHeight;
                    rowHeight = 0;

                    if (atlasPos.Y + glyphSurface.h > atlasSize.Y) {
                        Console.WriteLine($"Font atlas size provided for font {filepath} is too small!");
                        Environment.Exit(1);
                    }
                }

                for (int y = 0; y < glyphSurface.h; y++) {
                    for (int x = 0; x < glyphSurface.w; x++) {
                        for (int channel = 0; channel < 4; channel++) {
                            atlasData[((int) atlasPos.X + x + ((int) atlasPos.Y + y) * (int) atlasSize.X) * 4 + channel] = surfacePixels[(x * 4 + y * glyphSurface.pitch) + channel];
                        }
                    }
                }

                atlasPos.X += glyphSurface.w;
                rowHeight = Math.Max(rowHeight, glyphSurface.h);
            }

            Atlas = Texture.Create<byte>(atlasSize, TextureFormat.RgbaU8, atlasData);
        }

        ~Font() {
            SDL_ttf.TTF_CloseFont(sdlFont);
        }
    }
}
