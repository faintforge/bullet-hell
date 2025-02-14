using SDL2;

namespace BulletHell {
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

        public void ResetFrame() {
            foreach (SDL.SDL_Keycode key in keyboardMap.Keys.ToList()) {
                KeyState state = keyboardMap[key];
                state.IsFirstFrame = false;
                keyboardMap[key] = state;
            }
        }
    }
}
