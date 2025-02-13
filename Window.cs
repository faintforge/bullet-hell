using SDL2;
using OpenTK;
using OpenTK.Graphics;

namespace BulletHell {
    public class Window {
        private IntPtr sdlWindow;
        private IntPtr sdlGLContext;

        public bool Open { get; private set; } = true;

        public Window(string name, int width, int height, bool resizable = false, bool vsync = false) {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0) {
                Console.WriteLine("SDL failed to initialize!");
                Environment.Exit(1);
            }

            GLLoader.LoadBindings(new SDLGLLoader());
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

            SDL.SDL_WindowFlags flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;
            if (resizable) {
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            }

            sdlWindow = SDL.SDL_CreateWindow(
                    "Window",
                    SDL.SDL_WINDOWPOS_CENTERED,
                    SDL.SDL_WINDOWPOS_CENTERED,
                    800, 600,
                    flags
                );
            if (sdlWindow == IntPtr.Zero) {
                Console.WriteLine("SDL failed to create window!");
                Environment.Exit(1);
            }

            sdlGLContext = SDL.SDL_GL_CreateContext(sdlWindow);
            if (sdlGLContext == IntPtr.Zero) {
                Console.WriteLine("SDL failed to create GL context!");
                Environment.Exit(1);
            }
        }

        ~Window() {
            SDL.SDL_Quit();
        }

        public void PollEvents() {
            SDL.SDL_Event ev;
            while (SDL.SDL_PollEvent(out ev) == 1) {
                switch (ev.type) {
                    case SDL.SDL_EventType.SDL_QUIT:
                        Open = false;
                        break;
                }
            }
        }

        public void SwapBuffers() {
            SDL.SDL_GL_SwapWindow(sdlWindow);
        }
    }

    internal class SDLGLLoader : IBindingsContext {
        public IntPtr GetProcAddress(string procName)
        {
            return SDL.SDL_GL_GetProcAddress(procName);
        }
    }
}
