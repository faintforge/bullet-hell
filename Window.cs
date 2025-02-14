using SDL2;
using OpenTK;
using OpenTK.Graphics;

namespace BulletHell {
    public class Window {
        private IntPtr sdlWindow;
        private IntPtr sdlGLContext;
        private bool fullscreen;

        public bool Open { get; private set; } = true;
        public Vector2 Size { get; private set; }

        public Window(string name, int width, int height, bool resizable = false, bool vsync = false, bool fullscreen = false) {
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
            if (fullscreen) {
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP;
            }
            this.fullscreen = fullscreen;

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

            if (vsync) {
                SDL.SDL_GL_SetSwapInterval(1);
            } else {
                SDL.SDL_GL_SetSwapInterval(0);
            }

            Size = new Vector2(width, height);
        }

        ~Window() {
            SDL.SDL_Quit();
        }

        public void PollEvents() {
            SDL.SDL_Event ev;
            while (SDL.SDL_PollEvent(out ev) == 1) {
                HandleEvent(ev);
            }
        }

        private void HandleEvent(SDL.SDL_Event ev) {
            switch (ev.type) {
                case SDL.SDL_EventType.SDL_QUIT:
                    Open = false;
                    break;
                case SDL.SDL_EventType.SDL_WINDOWEVENT:
                    HandleWindowEvent(ev.window);
                    break;
                case SDL.SDL_EventType.SDL_KEYDOWN:
                case SDL.SDL_EventType.SDL_KEYUP: {
                    SDL.SDL_Keycode code = ev.key.keysym.sym;
                    if (ev.key.repeat == 1) {
                        break;
                    }
                    bool isDown = ev.type == SDL.SDL_EventType.SDL_KEYDOWN;
                    Input.Instance.SetKeyState(code, isDown);
                } break;
            }
        }

        private void HandleWindowEvent(SDL.SDL_WindowEvent ev) {
            switch (ev.windowEvent) {
                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                    Size = new Vector2(ev.data1, ev.data2);
                    break;
            }
        }

        public void SwapBuffers() {
            SDL.SDL_GL_SwapWindow(sdlWindow);
        }

        public void SetFullscreen(bool fullscreen) {
            if (this.fullscreen == fullscreen) {
                return;
            }

            if (fullscreen) {
                SDL.SDL_SetWindowFullscreen(sdlWindow, (uint) SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
            } else {
                SDL.SDL_SetWindowFullscreen(sdlWindow, 0);
            }

            this.fullscreen = fullscreen;
        }

        public void ToggleFullscreen() {
            SetFullscreen(!fullscreen);
        }
    }

    internal class SDLGLLoader : IBindingsContext {
        public IntPtr GetProcAddress(string procName)
        {
            return SDL.SDL_GL_GetProcAddress(procName);
        }
    }
}
