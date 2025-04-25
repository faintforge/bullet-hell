using SDL2;

namespace BulletHell {
    public enum MouseButton {
        Left,
        Middle,
        Right,
    }

    public class Input {
        private struct KeyState {
            public bool IsDown;
            public bool IsFirstFrame;
        }

        private static Input? instance = null;
        /// <summary>
        /// Singleton instance of this class.
        /// </summary>
        public static Input Instance {
            get {
                if (instance == null) {
                    instance = new Input();
                }
                return instance;
            }
        }

        private Dictionary<SDL.SDL_Keycode, KeyState> keyboardMap = new Dictionary<SDL.SDL_Keycode, KeyState>();
        private KeyState[] mouseState = new KeyState[Enum.GetNames(typeof(MouseButton)).Length];
        /// <summary>
        /// Mouse position in the window. (0, 0) is in the top left and Y increases down.
        /// </summary>
        public Vector2 MousePosition { get; internal set; }

        private Input() {}

        /// <summary>
        /// Get current state of key.
        /// </summary>
        /// <param name="code">Keycode.</param>
        /// <returns>True if key is pressed, false if not.</returns>
        public bool GetKey(SDL.SDL_Keycode code) {
            KeyState result;
            if (!keyboardMap.TryGetValue(code, out result)) {
                return false;
            }
            return result.IsDown;
        }

        /// <summary>
        /// Check if key was pressed this frame.
        /// </summary>
        /// <param name="code">Keycode.</param>
        /// <returns>True if key was pressed this frame, false if not.</returns>
        public bool GetKeyOnDown(SDL.SDL_Keycode code) {
            KeyState result;
            if (!keyboardMap.TryGetValue(code, out result)) {
                return false;
            }
            return result.IsDown && result.IsFirstFrame;
        }

        /// <summary>
        /// Check if key was released this frame.
        /// </summary>
        /// <param name="code">Keycode.</param>
        /// <returns>True if key was released this frame, false if not.</returns>
        public bool GetKeyOnUp(SDL.SDL_Keycode code) {
            KeyState result;
            if (!keyboardMap.TryGetValue(code, out result)) {
                return false;
            }
            return !result.IsDown && result.IsFirstFrame;
        }

        /// <summary>
        /// Internal function to set the state of a key. This is used to set state from the event handling code in the windowing system.
        /// </summary>
        /// <param name="code">Keycode.</param>
        /// <param name="isDown">Is the key down.</param>
        internal void SetKeyState(SDL.SDL_Keycode code, bool isDown) {
            keyboardMap[code] = new KeyState() {
                IsFirstFrame = true,
                IsDown = isDown,
            };
        }

        /// <summary>
        /// Get current state of a mouse button.
        /// </summary>
        /// <param name="button">Mouse button.</param>
        /// <returns>True if button is pressed, false if not.</returns>
        public bool GetButton(MouseButton button) {
            return mouseState[(int) button].IsDown;
        }

        /// <summary>
        /// Check if button was pressed this frame.
        /// </summary>
        /// <param name="button">Mouse button.</param>
        /// <returns>True if button was pressed this frame, false if not.</returns>
        public bool GetButtonOnDown(MouseButton button) {
            KeyState state = mouseState[(int) button];
            return state.IsDown && state.IsFirstFrame;
        }

        /// <summary>
        /// Check if button was released this frame.
        /// </summary>
        /// <param name="button">Mouse button.</param>
        /// <returns>True if button was released this frame, false if not.</returns>
        public bool GetButtonOnUp(MouseButton button) {
            KeyState state = mouseState[(int) button];
            return !state.IsDown && state.IsFirstFrame;
        }

        /// <summary>
        /// Internal function to set the state of a mouse button. This is used to set state from the event handling code in the windowing system.
        /// </summary>
        /// <param name="button">Mouse button.</param>
        /// <param name="isDown">Is the button down.</param>
        internal void SetButtonState(MouseButton button, bool isDown) {
            mouseState[(int) button] = new KeyState() {
                IsFirstFrame = true,
                IsDown = isDown,
            };
        }

        /// <summary>
        /// Resets the frame specific data in the input system. This should be called before polling for window events to work properly. If called after, OnDown and OnUp functions will never return true.
        /// </summary>
        public void ResetFrame() {
            foreach (SDL.SDL_Keycode key in keyboardMap.Keys.ToList()) {
                KeyState state = keyboardMap[key];
                state.IsFirstFrame = false;
                keyboardMap[key] = state;
            }

            for (int i = 0; i < mouseState.Length; i++) {
                mouseState[i].IsFirstFrame = false;
            }
        }
    }
}
