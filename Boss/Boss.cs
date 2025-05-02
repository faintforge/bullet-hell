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
// -----------------------------------------------------------------------------

namespace BulletHell {
    public class Boss : Enemy {
        private enum Phase {
            CrystalCluster,
            Vortex,
            Grid,
        }

        private Phase phase = Phase.CrystalCluster;
        private Player? target = null;
        private Random rng = new Random();

        private float attackTimer = 0.0f;

        private int attackCounter = 0;

        private float vortexAngle = 0.0f;
        private int vortexPoints = 2;

        public Boss(World world) : base(world) {
            Texture = AssetManager.Instance.GetTexture("boss");
            Transform.Size = Texture.Size;
            MaxHealth = 1000;
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

        private void SpawnGrid(Vector2 center, Vector2 gapSize) {
            if (target == null) {
                return;
            }

            Vector2 screenTL = world.Camera.ScreenToWorldSpace(new Vector2());
            Vector2 screenBR = world.Camera.ScreenToWorldSpace(world.Camera.ScreenSize);

            Vector2 screenSize = screenBR - screenTL;
            screenSize.Y = MathF.Abs(screenSize.Y);

            Vector2 segmentPerScreen = screenSize / 16.0f;
            Vector2 gridCount = screenSize / gapSize + 4.0f;
            gridCount /= 2.0f;
            Vector2 gridMin = -new Vector2(MathF.Floor(gridCount.X), MathF.Floor(gridCount.Y));
            Vector2 gridMax = new Vector2(MathF.Ceiling(gridCount.X), MathF.Ceiling(gridCount.Y));
            gridCount *= 2.0f;

            // Horizontal indicators
            for (float gridY = gridMin.Y; gridY < gridMax.Y; gridY++) {
                for (float x = MathF.Floor(gridMin.X * gapSize.X / 16.0f); x < MathF.Ceiling(gridMax.X * gapSize.X / 16.0f); x++) {
                    BeamIndicator indicatorSegment = world.SpawnEntity<BeamIndicator>();
                    indicatorSegment.Transform.Pos = center + new Vector2(x, 0.0f) * 16.0f + new Vector2(0.0f, gridY) * gapSize;;
                    indicatorSegment.Transform.Rot = MathF.PI / 2.0f;
                }
            }

            // Vertical indicators
            for (float gridX = gridMin.X; gridX < gridMax.X; gridX++) {
                for (float y = MathF.Floor(gridMin.Y * gapSize.Y / 16.0f); y < MathF.Ceiling(gridMax.Y * gapSize.Y / 16.0f); y++) {
                    BeamIndicator indicatorSegment = world.SpawnEntity<BeamIndicator>();
                    indicatorSegment.Transform.Pos = center + new Vector2(0.0f, y) * 16.0f + new Vector2(gridX, 0.0f) * gapSize;
                }
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
                        attackCounter++;
                    }

                    // Follow player.
                    Vector2 targetDir = target.Transform.Pos - Transform.Pos;
                    if (targetDir.MagnitudeSquared() != 0.0f) {
                        targetDir.Normalize();
                    }
                    Transform.Pos += targetDir * 50.0f * deltaTime;

                    // Go to next phase.
                    if (attackCounter == 4) {
                        attackCounter = 0;
                        phase = Phase.Vortex;
                    }
                } break;
                case Phase.Vortex: {
                    // Suck player
                    const float suchStrength = 50.0f;
                    Vector2 dir = target.Transform.Pos - Transform.Pos;
                    if (dir.MagnitudeSquared() != 0.0f) {
                        dir.Normalize();
                    }
                    target.Transform.Pos -= dir * suchStrength * deltaTime;

                    // Shoot spiral projectiles
                    const float rotationSpeed = 2.0f * MathF.PI / 36.0f;
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
                            vortexAngle += rotationSpeed;
                        } else {
                            vortexAngle -= rotationSpeed;
                        }
                    }
                    if (MathF.Abs(vortexAngle) >= MathF.PI * 2.0f) {
                        vortexAngle = 0.0f;
                        vortexPoints++;
                    }

                    float nextAngle;
                    if (vortexPoints % 2 == 0) {
                        nextAngle = vortexAngle + rotationSpeed;
                    } else {
                        nextAngle = vortexAngle - rotationSpeed;
                    }
                    Transform.Rot = vortexAngle + (nextAngle - vortexAngle) * attackTimer * 10.0f;

                    // Go to next phase.
                    if (vortexPoints == 5) {
                        vortexPoints = 2;
                        Transform.Rot = 0.0f;
                        phase = Phase.Grid;
                        attackTimer = 0.0f;
                    }
                } break;
                case Phase.Grid:
                {
                    attackTimer += deltaTime;
                    if (attackTimer >= 1.25f) {
                        attackTimer = 0.0f;
                        Vector2 gapSize = new Vector2(target.Transform.Size.Y) * 1.5f;
                        Vector2 offset = (new Vector2((float) rng.NextDouble(), (float) rng.NextDouble()) - 0.5f) * gapSize;
                        SpawnGrid(target.Transform.Pos + offset, gapSize);
                        attackCounter++;
                    }

                    if (attackCounter == 8) {
                        phase = Phase.CrystalCluster;
                        attackCounter = 0;
                        attackTimer = 0.0f;
                    }
                } break;
            }
        }

        public override void OnKill() {
            ParticleEmitter emitter = world.SpawnEntity<ParticleEmitter>();
            emitter.Cfg = new ParticleEmitter.Config() {
                Parent = this,
                SpawnRadius = 16.0f,
                SpawnAngle = 2.0f * MathF.PI,
                Color = Color.HexRGB(0xc65197),
                Size = new Vector2(2.0f),
                Count = 1000,
                Time = 0.0f,
                FinalSize = 0.5f,
                FinalOpacity = 0.0f,
                MinLifespan = 0.0f,
                MaxLifespan = 0.5f,
                VelocitySpeedMax = 100.0f,
            };
            emitter.Kill();

            // Spawn XP on death
            float spawnRadius = 32.0f;
            Random rng = new Random();
            for (int i = 0; i < 256; i++) {
                float angle = (float) rng.NextDouble() * 2.0f * MathF.PI;
                float distance = spawnRadius * MathF.Sqrt((float) rng.NextDouble());
                Vector2 pos = Vector2.FromAngle(angle) * distance;
                XpPoint xp = world.SpawnEntity<XpPoint>();
                xp.Transform.Pos = Transform.Pos + pos;
            }
        }
    }
}
