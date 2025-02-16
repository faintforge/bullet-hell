namespace BulletHell {
    [Flags] public enum EntityFlag {
        None        = 0,
        Player      = 1 << 0,
        Renderable  = 1 << 1,
    }

    public class Entity {
        public EntityFlag Flags;
        public string Name = "unnamed";

        // Transform
        public Vector2 Pos = new Vector2();
        public Vector2 Size = new Vector2(1.0f);
        public float Rot = 0.0f;
        public Vector2 Origin = new Vector2();

        // Player
        public float Speed = 25.0f;

        // Renderable
        public Texture? Texture = null;
        public Color Color = Color.WHITE;

        public Entity(EntityFlag flags) {
            Flags = flags;
        }
    }
}
