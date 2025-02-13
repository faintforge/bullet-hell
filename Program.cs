﻿using OpenTK.Graphics.OpenGL;
using SDL2;

namespace BulletHell {
    internal class Program {
        static void Main(string[] args) {
            Window window = new Window("Window", 800, 600, resizable: true);
            Renderer renderer = new Renderer();

            while (window.Open) {
                GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                renderer.BeginFrame(window.Size, 15.0f);
                renderer.Draw(new Vector2(0.0f, -1.0f), new Vector2(1.0f), Color.HSV(SDL.SDL_GetTicks() / 10, 0.75f, 1.0f));
                renderer.Draw(new Vector2(0.0f,  0.0f), new Vector2(1.0f), Color.WHITE);
                renderer.Draw(new Vector2(0.0f,  1.0f), new Vector2(1.0f), Color.BLACK);
                renderer.Draw(new Vector2(0.0f,  2.0f), new Vector2(1.0f), Color.RED);
                renderer.Draw(new Vector2(0.0f,  3.0f), new Vector2(1.0f), Color.GREEN);
                renderer.Draw(new Vector2(0.0f,  4.0f), new Vector2(1.0f), Color.BLUE);
                renderer.EndFrame();

                window.PollEvents();
                window.SwapBuffers();
            }
        }
    }
}
