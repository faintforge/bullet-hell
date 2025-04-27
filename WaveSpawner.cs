namespace BulletHell {
    public class WaveSpawner : Entity {
        public Player? Player { get; set; }

        private Random rng = new Random();
        private float waveTimer = 0.0f;
        private int wavePoints = 10;
        private int wave = 0;
        private Boss? boss;

        private struct Option {
            public int Cost { get; set; }
            public Type Type { get; set; }

            public Option(int cost, Type type) {
                Cost = cost;
                Type = type;
            }
        }
        private static Option[] options = new Option[] {
            new Option(1, typeof(Rat)),
            new Option(20, typeof(Goblin)),
        };
        private List<Enemy> aliveInWave = new List<Enemy>();

        public WaveSpawner(World world) : base(world) { }

        public override void OnSpawn() {
            SpawnWave();
        }

        public override void Update(float deltaTime) {
            if (boss != null && !boss.Alive) {
                boss = null;
            }

            aliveInWave.RemoveAll(enemy => !enemy.Alive);
            waveTimer += deltaTime;
            if ((waveTimer >= 30.0f || aliveInWave.Count == 0) && boss == null) {
                waveTimer = 0.0f;
                SpawnWave();
            }
        }

        private void SpawnWave() {
            if (wave >= 10 && wave % 10 == 0) {
                boss = world.SpawnEntity<Boss>();
            } else {
                int startingPoints = wavePoints;
                while (wavePoints > 0) {
                    // Find highest costing enemy
                    int bestEnemy = options.Length;
                    for (int i = 0; i < options.Length; i++) {
                        if (options[i].Cost > wavePoints) {
                            bestEnemy = i;
                            break;
                        }
                    }

                    // Pick a random spot to spawn enemy
                    float angle = (float) rng.NextDouble() * MathF.PI * 2.0f;
                    float distance = 256.0f + (float) rng.NextDouble() * 360.0f;
                    Vector2 enemyPos = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * distance;

                    // Spawn a random enemy within budget
                    int index = rng.Next(bestEnemy);
                    Option option = options[index];
                    Enemy enemy = (Enemy) world.SpawnEntity(option.Type);
                    enemy.Target = Player;
                    enemy.Transform.Pos = enemyPos;
                    aliveInWave.Add(enemy);

                    wavePoints -= option.Cost;
                }

                wavePoints = (int) MathF.Ceiling(startingPoints * 1.10f) + 10;
            }

            wave++;
        }
    }
}
