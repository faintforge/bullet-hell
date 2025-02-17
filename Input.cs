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
        public Vector2 MousePosition { get; internal set; }

        private Input() {}

        public bool GetKey(SDL.SDL_Keycode code) {
            KeyState result;
            if (!keyboardMap.TryGetValue(code, out result)) {
                return false;
            }
            return result.IsDown;
        }

        public bool GetKeyOnDown(SDL.SDL_Keycode code) {
            KeyState result;
            if (!keyboardMap.TryGetValue(code, out result)) {
                return false;
            }
            return result.IsDown && result.IsFirstFrame;
        }

        public bool GetKeyOnUp(SDL.SDL_Keycode code) {
            KeyState result;
            if (!keyboardMap.TryGetValue(code, out result)) {
                return false;
            }
            return !result.IsDown && result.IsFirstFrame;
        }

        internal void SetKeyState(SDL.SDL_Keycode code, bool isDown) {
            keyboardMap[code] = new KeyState() {
                IsFirstFrame = true,
                IsDown = isDown,
            };
        }

        public bool GetButton(MouseButton button) {
            return mouseState[(int) button].IsDown;
        }

        public bool GetButtonOnDown(MouseButton button) {
            KeyState state = mouseState[(int) button];
            return state.IsDown && state.IsFirstFrame;
        }

        public bool GetButtonOnUp(MouseButton button) {
            KeyState state = mouseState[(int) button];
            return !state.IsDown && state.IsFirstFrame;
        }

        internal void SetButtonState(MouseButton button, bool isDown) {
            mouseState[(int) button] = new KeyState() {
                IsFirstFrame = true,
                IsDown = isDown,
            };
        }

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
