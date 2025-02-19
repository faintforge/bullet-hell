using OpenTK.Graphics.OpenGL;

namespace BulletHell {
    public class Game {
        private Window window;
        private Renderer renderer;
        private World world = new World();
        private Player player;

        public Game(Window window, Renderer renderer) {
            this.window = window;
            this.renderer = renderer;

            world.Camera.Zoom = 50.0f;
            player = world.SpawnEntity<Player>();
        }

        public void Run(float deltaTime) {
            GL.Viewport(0, 0, (int) window.Size.X, (int) window.Size.Y);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            world.Update(deltaTime);
            world.Camera.ScreenSize = window.Size;
            renderer.BeginFrame(world.Camera);
            world.OperateOnEntities((entity) => {
                renderer.Draw(
                        entity.Transform,
                        entity.Color,
                        entity.Texture);
            });
            renderer.EndFrame();

            // Camera uiCam = new Camera(window.Size, window.Size / 2.0f, window.Size.Y, true);
            // renderer.BeginFrame(uiCam);
            // Font font = AssetManager.Instance.GetFont("lato32");
            // renderer.DrawText($"{player.Transform.Pos.X}, {player.Transform.Pos.Y}", font, new Vector2(8.0f), Color.WHITE);
            // renderer.EndFrame();
        }
    }
}
