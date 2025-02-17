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
        public Box Transform = new Box() {
            Pos = new Vector2(),
            Size = new Vector2(1.0f),
            Rot = 0.0f,
            Origin = new Vector2(),
        };

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
