namespace BulletHell {
    public class GoblinSpawner : Entity {
        private Player? player;
        private float spawnTimer = 0.0f;

        public GoblinSpawner(World world) : base(world) {}

        public override void Update(float deltaTime) {
            if (player == null) {
                world.OperateOnEntities((entity) => {
                        if (entity is Player) {
                        player = (Player) entity;
                    }
                });
                if (player == null) {
                    return;
                }
            }

            spawnTimer += deltaTime;
            if (spawnTimer >= 1.0f) {
                spawnTimer = 0.0f;

                Random rng = new Random();
                float angle = (float) rng.NextDouble() * MathF.PI * 2.0f;
                Vector2 pos = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 360.0f;;

                Goblin goblin = world.SpawnEntity<Goblin>();
                goblin.Target = player;
                goblin.Transform.Pos = pos;
            }
        }
    }
}
