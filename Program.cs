using OpenTK.Graphics.OpenGL;

namespace BulletHell {
    internal class Program {
        static void Main(string[] args) {
            Window window = new Window("Window", 800, 600);

            while (window.Open) {
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                window.PollEvents();
                window.SwapBuffers();
            }
        }
    }
}
