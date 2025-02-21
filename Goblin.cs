namespace BulletHell {
    public class Goblin : Enemy {
        public Player? Target { get; set; }
        private const float shootDelay = 2.0f;
        private float shootTimer = 0.0f;
        private float speed = 50.0f;

        public Goblin(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("goblin");
            Transform.Size = Texture.Size;
            MaxHealth = 10;
        }

        public override void AI(float deltaTime) {
            // if (Target == null) {
            //     world.OperateOnEntities((entity) => {
            //         if (entity is Player) {
            //             Target = (Player) entity;
            //         }
            //     });
            //     if (Target == null) {
            //         return;
            //     }
            // }

            if (Target == null) {
                return;
            }

            Vector2 dir = Target.Transform.Pos - Transform.Pos;
            dir.Normalize();

            Transform.Pos += dir * speed * deltaTime;

            shootTimer += deltaTime;
            if (shootTimer >= shootDelay) {
                shootTimer = 0.0f;

                EnemyDagger dagger = world.SpawnEntity<EnemyDagger>();
                dagger.Transform.Pos = Transform.Pos;
                dagger.Transform.Rot = MathF.Atan2(dir.Y, dir.X) - MathF.PI / 2.0f;
                dagger.Velocity = dir * 150.0f;
            }
        }
    }
}
