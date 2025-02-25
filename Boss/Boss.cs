namespace BulletHell {
    public class Boss : Enemy {
        public Boss(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("boss");
            Transform.Size = Texture.Size;
            MaxHealth = 100;

            int count = 8;
            float angle = (MathF.PI * 2.0f) / count;
            for (int i = 0; i < count; i++) {
                float currentAngle = angle * i;
                Vector2 dir = new Vector2(MathF.Cos(currentAngle), MathF.Sin(currentAngle));
                SpawnCrystalCluster(dir * 256.0f, 6, 16.0f);
            }
        }

        public void SpawnCrystalCluster(Vector2 position, int count, float space) {
            float angle = (MathF.PI * 2.0f) / count;
            for (int i = 0; i < count; i++) {
                float currentAngle = angle * i;
                Vector2 dir = new Vector2(MathF.Cos(currentAngle), MathF.Sin(currentAngle));

                CrystalShard shard = world.SpawnEntity<CrystalShard>();
                shard.Transform.Pos = dir * space + position;
                shard.Transform.Rot = currentAngle + MathF.PI / 2.0f;
                shard.Velocity = dir * 300.0f;
            }
        }
    }
}
