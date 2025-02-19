using SDL2;
using System.Runtime.InteropServices;

namespace BulletHell {
    public struct Glyph {
        public Vector2 Size;
        public Vector2 Offset;
        public Vector2[] UVs;
        public float Advance;
    }

    public struct FontMetrics {
        public float Ascent;
        public float Descent;
        public float LineGap;
    }

    public class Font {
        public Texture Atlas { get; private set; }
        IntPtr sdlFont;

        const byte ASCII_START = 32;
        const byte ASCII_END = 126;

        private Glyph[] glyphs = new Glyph[ASCII_END - ASCII_START + 1];

        public Font(string filepath, int size, Vector2 atlasSize) {
            sdlFont = SDL_ttf.TTF_OpenFont(filepath, size);
            if (sdlFont == IntPtr.Zero) {
                Console.WriteLine($"Failed to load font {filepath}!");
                Environment.Exit(1);
            }

            byte[] atlasData = new byte[(int) (atlasSize.X * atlasSize.Y) * 4];

            Vector2 atlasPos = new Vector2();
            float rowHeight = 0;
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

                Vector2 glyphPos = atlasPos;
                Vector2 glyphSize = new Vector2(glyphSurface.w, glyphSurface.h);

                atlasPos.X += glyphSurface.w;
                rowHeight = Math.Max(rowHeight, glyphSurface.h);

                int x0, y0;
                int x1, y1;
                int advance;
                SDL_ttf.TTF_GlyphMetrics(sdlFont, c, out x0, out x1, out y0, out y1, out advance);

                glyphs[c - ASCII_START] = new Glyph() {
                    Advance = advance,
                    Size = glyphSize,
                    Offset = new Vector2(x0, y0),
                    UVs = new Vector2[2] {
                        glyphPos / atlasSize,
                        (glyphPos + glyphSize) / atlasSize,
                    },
                };
            }

            Atlas = Texture.Create<byte>(atlasSize, TextureFormat.RgbaU8, atlasData);
        }

        ~Font() {
            SDL_ttf.TTF_CloseFont(sdlFont);
        }

        public Glyph GetGlyph(char c) {
            return glyphs[c - ASCII_START];
        }

        public FontMetrics GetMetrics() {
            return new FontMetrics() {
                Ascent = SDL_ttf.TTF_FontAscent(sdlFont),
                Descent = SDL_ttf.TTF_FontDescent(sdlFont),
                LineGap = SDL_ttf.TTF_FontLineSkip(sdlFont),
            };
        }

        public Vector2 MeasureText(string text) {
            FontMetrics metrics = GetMetrics();
            Vector2 size = new Vector2(0.0f, metrics.Ascent - metrics.Descent);
            foreach (char c in text) {
                Glyph glyph = GetGlyph(c);
                size.X += glyph.Advance;
            }
            return size;
        }
    }
}
