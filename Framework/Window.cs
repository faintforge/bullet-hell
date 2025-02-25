using SDL2;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BulletHell {
    public class Window {
        private class SDLGLLoader : IBindingsContext {
            public IntPtr GetProcAddress(string procName)
            {
                return SDL.SDL_GL_GetProcAddress(procName);
            }
        }

        private IntPtr sdlWindow;
        private IntPtr sdlGLContext;
        private bool fullscreen;

        public bool Open { get; private set; } = true;
        public Vector2 Size { get; private set; }

        /// <summary>
        /// Create a window.
        /// </summary>
        /// <param name="name">Name/title of the window.</param>
        /// <param name="width">Width of window.</param>
        /// <param name="height">Height of window.</param>
        /// <param name="resizable">Is the window resizable</param>
        /// <param name="vsync">Is the window rendering with vsync.</param>
        /// <param name="fullscreen">Is the window fullscreen.</param>
        public Window(string name, int width, int height, bool resizable = false, bool vsync = false, bool fullscreen = false) {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0) {
                Console.WriteLine("SDL failed to initialize!");
                Environment.Exit(1);
            }

            if (SDL_ttf.TTF_Init() < 0) {
                Console.WriteLine("SDL failed to initialize TTF subsystem!");
                Environment.Exit(1);
            }

            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0) {
                Console.WriteLine("SDL failed to initialize image subsystem!");
                Environment.Exit(1);
            }

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, 4);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 4);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 6);
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
                    name,
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

            GL.LoadBindings(new SDLGLLoader());
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            // GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            // GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            if (vsync) {
                SDL.SDL_GL_SetSwapInterval(1);
            } else {
                SDL.SDL_GL_SetSwapInterval(0);
            }

            Size = new Vector2(width, height);
        }

        ~Window() {
            SDL_ttf.TTF_Quit();
            SDL.SDL_Quit();
        }

        /// <summary>
        /// Poll window events sent by the operating system.
        /// </summary>
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
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP: {
                    uint sdlButton = ev.button.button;
                    MouseButton button = MouseButton.Left;
                    switch (sdlButton) {
                        case SDL.SDL_BUTTON_LEFT:
                            button = MouseButton.Left;
                            break;
                        case SDL.SDL_BUTTON_MIDDLE:
                            button = MouseButton.Middle;
                            break;
                        case SDL.SDL_BUTTON_RIGHT:
                            button = MouseButton.Right;
                            break;
                    }
                    bool isDown = ev.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN;
                    Input.Instance.SetButtonState(button, isDown);
                } break;
                case SDL.SDL_EventType.SDL_MOUSEMOTION: {
                    Input.Instance.MousePosition = new Vector2(ev.motion.x, ev.motion.y);
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

        /// <summary>
        /// Swap the front and back buffers of the window presenting the currently rendered frame.
        /// </summary>
        public void SwapBuffers() {
            SDL.SDL_GL_SwapWindow(sdlWindow);
        }

        /// <summary>
        /// Set the fullscreen state of the window.
        /// </summary>
        /// <param name="fullscreen">Fullscreen value.</param>
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

        /// <summary>
        /// Toggle fullscreen. If window is fullscreen it becoems windowed and vice versa.
        /// </summary>
        public void ToggleFullscreen() {
            SetFullscreen(!fullscreen);
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        public void Close() {
            Open = false;
        }
    }
}
