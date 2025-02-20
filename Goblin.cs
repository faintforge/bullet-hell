namespace BulletHell {
    public class Goblin : Enemy {
        private Player? target = null;
        private const float shootDelay = 0.5f;
        private float shootTimer = 0.0f;
        private float speed = 50.0f;

        public Goblin(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("goblin");
            Transform.Size = Texture.Size;
            MaxHealth = 100;
        }

        public override void AI(float deltaTime) {
            if (target == null) {
                List<Entity> near = world.SpatialQuery(Transform.Pos, 150.0f);
                foreach (Entity entity in near) {
                    if (entity is Player) {
                        target = (Player) entity;
                    }
                }

                if (target == null) {
                    return;
                }
            }

            Vector2 dir = target.Transform.Pos - Transform.Pos;
            dir.Normalize();

            Transform.Pos += dir * speed * deltaTime;

            shootTimer += deltaTime;
            if (shootTimer >= shootDelay) {
                shootTimer = 0.0f;

                EnemyDagger dagger = world.SpawnEntity<EnemyDagger>();
                dagger.Transform.Pos = Transform.Pos;
                dagger.Transform.Rot = (float) Math.Atan2(dir.Y, dir.X) - (float) Math.PI / 2.0f;
                dagger.Velocity = dir * 150.0f;
            }
        }

        public override void OnKill() {
            Goblin goblin = world.SpawnEntity<Goblin>();
            goblin.Transform.Pos = Transform.Pos + new Vector2(1.0f) * -8.0f;

            goblin = world.SpawnEntity<Goblin>();
            goblin.Transform.Pos = Transform.Pos + new Vector2(1.0f) * 8.0f;
        }
    }
}
