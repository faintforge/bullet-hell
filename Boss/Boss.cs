// --- Attack pattern ----------------------------------------------------------
// 1. Spwans n number of clusters around the player with a delay
// between each set of n.
//
// 2. Sucks in the player while shoting out bullets in a spiral pattern.
//
// 3. Stay still while creating obstacles for the player like beams.
// +---+---+---+---+
// |   |   |   |   |
// |   |   |   |   |
// +---+---+---+---+
// |   |   |   |   |
// |   |   |   |   |
// +---+---+---+---+
// Then it does the same with diagonal lines so the player has to stay in the
// little squares.
// -----------------------------------------------------------------------------

namespace BulletHell {
    public class Boss : Enemy {
        private enum Phase {
            Idle,
            CrystalCluster,
            Vortex,
        }

        private Phase phase = Phase.CrystalCluster;
        private Player? target = null;
        private Random rng = new Random();

        private float attackTimer = 0.0f;

        private int clusterSpawnCounter = 0;

        private float vortexAngle = 0.0f;
        private int vortexPoints = 2;

        public Boss(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("boss");
            Transform.Size = Texture.Size;
            MaxHealth = 100;
        }

        public void SpawnCrystalCluster(Vector2 position, int count, float space) {
            float angle = (MathF.PI * 2.0f) / count;
            for (int i = 0; i < count; i++) {
                float currentAngle = angle * i;
                Vector2 dir = new Vector2(MathF.Cos(currentAngle), MathF.Sin(currentAngle));

                CrystalClusterShard shard = world.SpawnEntity<CrystalClusterShard>();
                shard.Transform.Pos = dir * space + position;
                shard.Transform.Rot = currentAngle + MathF.PI / 2.0f;
                shard.Velocity = dir * 300.0f;
            }
        }

        public override void AI(float deltaTime) {
            if (target == null) {
                world.OperateOnEntities((entity) => {
                    if (entity is Player) {
                        target = (Player) entity;
                    }
                });

                if (target == null) {
                    return;
                }
            }

            switch (phase) {
                case Phase.CrystalCluster: {
                    // Spwan crystal clusters.
                    attackTimer += deltaTime;
                    if (attackTimer >= 2.0f) {
                        attackTimer = 0.0f;
                        const int count = 5;
                        float angle = (MathF.PI * 2.0f) / count;
                        float currentAngle = 0.0f;
                        for (int i = 0; i < count; i++) {
                            currentAngle += angle * ((float) rng.NextDouble() / 2.0f + 0.5f);
                            Vector2 dir = Vector2.FromAngle(currentAngle) * ((float) rng.NextDouble() / 2.0f + 0.5f);
                            SpawnCrystalCluster(target.Transform.Pos + dir * 256.0f, 8, 16.0f);
                        }
                        clusterSpawnCounter++;
                    }

                    // Follow player.
                    Vector2 targetDir = target.Transform.Pos - Transform.Pos;
                    if (targetDir.MagnitudeSquared() != 0.0f) {
                        targetDir.Normalize();
                    }
                    Transform.Pos += targetDir * 50.0f * deltaTime;

                    // Go to next phase.
                    if (clusterSpawnCounter == 4) {
                        clusterSpawnCounter = 0;
                        phase = Phase.Vortex;
                    }
                } break;
                case Phase.Vortex: {
                    // Suck player
                    const float suchStrength = 75.0f;
                    Vector2 dir = target.Transform.Pos - Transform.Pos;
                    if (dir.MagnitudeSquared() != 0.0f) {
                        dir.Normalize();
                    }
                    target.Transform.Pos -= dir * suchStrength * deltaTime;

                    // Shoot spiral projectiles
                    attackTimer += deltaTime;
                    if (attackTimer >= 0.10f) {
                        attackTimer = 0.0f;
                        for (int i = 0; i < vortexPoints; i++) {
                            float angle = vortexAngle + (2.0f * MathF.PI / vortexPoints) * i;
                            CrystalShard shard = world.SpawnEntity<CrystalShard>();
                            shard.Transform.Pos = Transform.Pos;
                            shard.Velocity = Vector2.FromAngle(angle) * 200.0f;
                            shard.Transform.Rot = angle + MathF.PI / 2.0f;
                        }
                        if (vortexPoints % 2 == 0) {
                            vortexAngle += 2.0f * MathF.PI / 36.0f;
                        } else {
                            vortexAngle -= 2.0f * MathF.PI / 36.0f;
                        }
                    }
                    if (MathF.Abs(vortexAngle) >= MathF.PI * 2.0f) {
                        vortexAngle = 0.0f;
                        vortexPoints++;
                    }

                    // Go to next phase.
                    if (vortexPoints == 5) {
                        vortexPoints = 2;
                        phase = Phase.Idle;
                    }
                } break;
            }
        }
    }
}
