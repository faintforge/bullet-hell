using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace BulletHell {
    public class Renderer {
        [StructLayout(LayoutKind.Sequential)]
        private struct Vertex {
            public Vector2 Pos { get; set; } = new Vector2();
            public Vector2 UV { get; set; } = new Vector2();
            public Color Color { get; set; } = new Color();
            public float textureIndex { get; set; } = 0;

            public Vertex() {}
        }

        private int vertexArray;
        private int vertexBuffer;
        private int indexBuffer;
        private int maxQuadCount = 0;
        private int currentQuad = 0;
        private Shader shader;
        private Matrix4 projection;
        private Vector2 cameraPos;
        private Texture[] textures = new Texture[32];
        private int currentTexture = 1;
        private Vector2 screenSize;
        private float zoom;

        private Vertex[] vertices;

        public Renderer(int maxQuadCount = 4096) {
            this.maxQuadCount = maxQuadCount;
            vertices = new Vertex[maxQuadCount * 6];

            vertexArray = GL.CreateVertexArray();
            GL.BindVertexArray(vertexArray);

            vertexBuffer = GL.CreateBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, maxQuadCount * 6 * Marshal.SizeOf<Vertex>(), IntPtr.Zero, BufferUsage.DynamicDraw);

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

            indexBuffer = GL.CreateBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            uint[] indices = new uint[maxQuadCount * 6];
            uint j = 0;
            for (int i = 0; i < maxQuadCount; i++) {
                indices[i * 6 + 0] = j + 0;
                indices[i * 6 + 1] = j + 1;
                indices[i * 6 + 2] = j + 2;
                indices[i * 6 + 3] = j + 2;
                indices[i * 6 + 4] = j + 3;
                indices[i * 6 + 5] = j + 1;
                j += 4;
            }
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsage.StaticDraw);

            shader = Shader.FromFile("assets/batch.vert.glsl", "assets/batch.frag.glsl");
            int[] samplers = new int[32];
            for (int i = 0; i < samplers.Length; i++) {
                samplers[i] = i;
            }
            shader.Use();
            shader.UniformInt("textures", samplers);

            byte[] pixels = {255, 255, 255, 255};
            textures[0] = Texture.Create<byte>(new Vector2(1.0f), TextureFormat.RgbaU8, pixels);
        }

        ~Renderer() {
            GL.DeleteVertexArray(vertexArray);
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indexBuffer);
        }

        public void BeginFrame(Vector2 screenSize, float zoom, Vector2 cameraPos) {
            this.screenSize = screenSize;
            this.zoom = zoom;
            this.cameraPos = cameraPos;

            currentQuad = 0;
            currentTexture = 1;

            zoom /= 2.0f;
            float aspect = screenSize.X / screenSize.Y * zoom;
            projection = Matrix4.OrthographicProjection(-aspect, aspect, zoom, -zoom, -1.0f, 1.0f);
        }

        public void EndFrame() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferSubData<Vertex>(BufferTarget.ArrayBuffer, 0, Marshal.SizeOf<Vertex>() * currentQuad * 4, vertices);

            shader.Use();
            shader.UniformMatrix4("projection", projection);
            for (uint i = 0; i < currentTexture; i++) {
                textures[i].Bind(i);
            }
            GL.BindVertexArray(vertexArray);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, currentQuad * 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void Draw(Vector2 pos, Vector2 size, Color color, Texture? texture = null) {
            if (currentQuad == maxQuadCount || currentTexture == textures.Length) {
                EndFrame();
                BeginFrame(screenSize, zoom, cameraPos);
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

            Vector2[] vertPos = [
                new Vector2(-0.5f, -0.5f),
                new Vector2( 0.5f, -0.5f),
                new Vector2(-0.5f,  0.5f),
                new Vector2( 0.5f,  0.5f),
            ];
            Vector2[] vertUV = [
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
            ];

            for (int i = 0; i < 4; i++) {
                ref Vertex vert = ref vertices[currentQuad * 4 + i];
                vert.Pos = vertPos[i];
                vert.Pos *= size;
                vert.Pos += pos;
                vert.Pos -= cameraPos;

                vert.UV = vertUV[i];
                vert.Color = color;
                vert.textureIndex = textureIndex;
            }

            currentQuad++;
        }
    }
}
