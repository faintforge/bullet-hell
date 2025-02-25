using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace BulletHell {
    public class Renderer {
        [StructLayout(LayoutKind.Sequential)]
        private struct Vertex {
            public Vector2 Pos { get; set; } = new Vector2();
            public Vector2 UV { get; set; } = new Vector2();
            public Color Color { get; set; } = Color.WHITE;
            public float textureIndex { get; set; } = 0;

            public Vertex() {}
        }

        private int vertexArray;
        private int vertexBuffer;
        private int indexBuffer;
        private int maxQuadCount = 0;
        private int currentQuad = 0;
        private Shader shader;
        private Texture[] textures = new Texture[32];
        private int currentTexture = 1;
        private Camera cam;

        private Vertex[] vertices;

        /// <summary>
        /// Create a batch renderer.
        /// </summary>
        /// <param name="maxQuadCount">Max amount of quads in a single batch.</param>
        public Renderer(int maxQuadCount = 4096) {
            this.maxQuadCount = maxQuadCount;
            vertices = new Vertex[maxQuadCount * 6];

            GL.CreateVertexArrays(1, out vertexArray);
            GL.BindVertexArray(vertexArray);

            GL.CreateBuffers(1, out vertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, maxQuadCount * 6 * Marshal.SizeOf<Vertex>(), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            // Position
            GL.VertexAttribPointer(
                    0,
                    2,
                    VertexAttribPointerType.Float,
                    false,
                    Marshal.SizeOf<Vertex>(),
                    0
                );
            GL.EnableVertexAttribArray(0);

            // UV
            GL.VertexAttribPointer(
                    1,
                    2,
                    VertexAttribPointerType.Float,
                    false,
                    Marshal.SizeOf<Vertex>(),
                    Marshal.SizeOf<Vector2>()
                );
            GL.EnableVertexAttribArray(1);

            // Color
            GL.VertexAttribPointer(
                    2,
                    4,
                    VertexAttribPointerType.Float,
                    false,
                    Marshal.SizeOf<Vertex>(),
                    Marshal.SizeOf<Vector2>() * 2
                );
            GL.EnableVertexAttribArray(2);

            // Color
            GL.VertexAttribPointer(
                    3,
                    1,
                    VertexAttribPointerType.Float,
                    false,
                    Marshal.SizeOf<Vertex>(),
                    Marshal.SizeOf<Vector2>() * 2 + Marshal.SizeOf<Color>()
                );
            GL.EnableVertexAttribArray(3);

            GL.CreateBuffers(1, out indexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            uint[] indices = new uint[maxQuadCount * 6];
            uint j = 0;
            for (int i = 0; i < maxQuadCount; i++) {
                indices[i * 6 + 0] = j + 0;
                indices[i * 6 + 1] = j + 1;
                indices[i * 6 + 2] = j + 2;
                indices[i * 6 + 3] = j + 2;
                indices[i * 6 + 4] = j + 3;
                indices[i * 6 + 5] = j + 0;
                j += 4;
            }
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = Shader.FromFile("assets/batch.vert.glsl", "assets/batch.frag.glsl");
            int[] samplers = new int[32];
            for (int i = 0; i < samplers.Length; i++) {
                samplers[i] = i;
            }
            shader.Use();
            shader.UniformInt("textures", samplers);

            byte[] pixels = {255, 255, 255, 255};
            textures[0] = Texture.Create<byte>(new Vector2(1.0f), TextureFormat.RgbaU8, pixels);

            cam = new Camera(new Vector2(), new Vector2(), 0.0f);
        }

        ~Renderer() {
            GL.DeleteVertexArray(vertexArray);
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indexBuffer);
        }

        /// <summary>
        /// Prepare the batch renderer to render a frame.
        /// </summary>
        /// <param name="cam">Camera to use for rendering this frame.</param>
        public void BeginFrame(Camera cam) {
            this.cam = cam;
            currentQuad = 0;
            currentTexture = 1;
        }

        /// <summary>
        /// End the frame. Render current batch to the screeen.
        /// </summary>
        public void EndFrame() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferSubData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr) 0, Marshal.SizeOf<Vertex>() * currentQuad * 4, vertices);

            shader.Use();
            shader.UniformMatrix4("projection", cam.Projection);
            for (uint i = 0; i < currentTexture; i++) {
                textures[i].Bind(i);
            }
            GL.BindVertexArray(vertexArray);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, currentQuad * 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        /// <summary>
        /// Draw a quad to screen with specified UV coordinates.
        /// </summary>
        /// <param name="box">Quad information.</param>
        /// <param name="color">Color.</param>
        /// <param name="texture">Texture.</param>
        /// <param name="uvTopLeft">Top left UV coordinate.</param>
        /// <param name="uvBottomRight">Bottom right UV coordinate.</param>
        public void DrawUV(Box box, Color color, Texture? texture, Vector2 uvTopLeft, Vector2 uvBottomRight) {
            if (currentQuad == maxQuadCount || currentTexture == textures.Length) {
                EndFrame();
                BeginFrame(cam);
            }

            float textureIndex = 0;
            if (texture != null) {
                for (int i = 0; i < currentTexture; i++) {
                    if (texture == textures[i]) {
                        textureIndex = i;
                    }
                }

                if (textureIndex == 0) {
                    textures[currentTexture] = texture;
                    textureIndex = currentTexture;
                    currentTexture++;
                }
            }

            Vector2 camPos = cam.Position;
            if (cam.InvertY) {
                Vector2 boxPos = box.Pos;
                boxPos.Y = -boxPos.Y;
                box.Pos = boxPos;

                Vector2 boxOrigin = box.Origin;
                boxOrigin.Y = -boxOrigin.Y;
                box.Origin = boxOrigin;

                camPos.Y = -camPos.Y;
            }
            Vector2[] vertPos = box.GetVertices();

            float left = uvTopLeft.X;
            float top = uvTopLeft.Y;
            float right = uvBottomRight.X;
            float bottom = uvBottomRight.Y;
            Vector2[] vertUV = {
                new Vector2(left,  top),
                new Vector2(right, top),
                new Vector2(right, bottom),
                new Vector2(left,  bottom),
            };

            for (int i = 0; i < 4; i++) {
                ref Vertex vert = ref vertices[currentQuad * 4 + i];
                vert.Pos = vertPos[i];
                vert.Pos -= camPos;

                vert.UV = vertUV[i];
                vert.Color = color;
                vert.textureIndex = textureIndex;
            }

            currentQuad++;
        }

        /// <summary>
        /// Draw a quad to screen.
        /// </summary>
        /// <param name="box">Quad information.</param>
        /// <param name="color">Color.</param>
        /// <param name="texture">Texture. If null then a solid colored quad will be drawn.</param>
        public void Draw(Box box, Color color, Texture? texture = null) {
            DrawUV(box, color, texture, new Vector2(0.0f), new Vector2(1.0f));
        }

        /// <summary>
        /// Draw text to screen. Use a screen space camera that uses screen coordinates to draw in order to use this function.
        /// </summary>
        /// <param name="text">String to draw.</param>
        /// <param name="font">Font to use.</param>
        /// <param name="position">Top left position of text.</param>
        /// <param name="color">Color of text.</param>
        /// <returns>Size of drawn width.</returns>
        public Vector2 DrawText(string text, Font font, Vector2 position, Color color) {
            Vector2 glyphPos = position;
            foreach (char c in text) {
                Glyph g = font.GetGlyph(c);
                DrawUV(
                        new Box() {
                        Origin = new Vector2(-1.0f),
                        Pos = glyphPos,
                        Size = g.Size,
                        }, color, font.Atlas, g.UVs[0], g.UVs[1]);
                glyphPos.X += g.Advance;
            }
            float width = glyphPos.X - position.X;
            return new Vector2(width, font.GetMetrics().Ascent - font.GetMetrics().Descent);
        }
    }
}
