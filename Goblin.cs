namespace BulletHell {
    public class Goblin : Enemy {
        private Player? target = null;
        private const float shootDelay = 0.5f;
        private float shootTimer = 0.0f;

        public Goblin(World world) : base(world) {
            Color = Color.HexRGB(0x75a743);
            MaxHealth = 100;
        }

        public override void AI(float deltaTime) {
            if (target == null) {
                List<Entity> near = world.SpatialQuery(Transform.Pos, 15.0f);
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

            Transform.Pos += dir * 10.0f * deltaTime;

            shootTimer += deltaTime;
            if (shootTimer >= shootDelay) {
                shootTimer = 0.0f;

                EnemyDagger dagger = world.SpawnEntity<EnemyDagger>();
                dagger.Transform.Pos = Transform.Pos;
                dagger.Transform.Rot = (float) Math.Atan2(dir.Y, dir.X);
                dagger.Velocity = dir * 30.0f;
            }
        }

        public override void OnHit(Projectile projectile, int damage) {
            Console.WriteLine($"Ouch {damage}");
        }
    }
}
