using SDL2;
using System.Reflection;

namespace BulletHell {
    public class Player : Entity {
        public float Speed { get; set; } = 100.0f;
        private float shootTimer = 0.0f;
        public float ShootDelay { get; set; } = 0.5f;
        public int MaxHealth { get; set; } = 100;
        public int Health { get; set; }
        public int NeededXp { get; set; } = 50;
        public int Xp { get; set; } = 0;
        public bool LeveledWithoutUpgrade { get; set; } = false;
        public Upgrade[] Upgrades { get; private set; } = new Upgrade[3];
        public int Pierce { get; set; } = 1;
        public float DamageMod { get; set; } = 1.0f;

        public Player(World world) : base(world) {
            Render = true;
            Collider = true;
            Texture = AssetManager.Instance.GetTexture("player");
            float asepctRatio = Texture.Size.X / Texture.Size.Y;
            Transform.Size = Texture.Size;
        }

        public override void OnSpawn() {
            Health = MaxHealth;
        }

        public override void Update(float deltaTime) {
            Vector2 vel = new Vector2();
            vel.X -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_a));
            vel.X += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_d));
            vel.Y -= Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_s));
            vel.Y += Convert.ToInt32(Input.Instance.GetKey(SDL.SDL_Keycode.SDLK_w));
            if (vel.MagnitudeSquared() != 0.0f) {
                vel.Normalize();
            }
            vel *= Speed;
            Transform.Pos += vel * deltaTime;

            // Lerp camera to player position
            world.Camera.Position = new Vector2(
                    world.Camera.Position.X + (Transform.Pos.X - world.Camera.Position.X) * deltaTime * 5.0f,
                    world.Camera.Position.Y + (Transform.Pos.Y - world.Camera.Position.Y) * deltaTime * 5.0f);

            shootTimer += deltaTime;
            if (Input.Instance.GetButton(MouseButton.Left) && shootTimer >= ShootDelay) {
                shootTimer = 0.0f;
                Projectile proj = world.SpawnEntity<FireBolt>();
                proj.Damage = (int) (proj.Damage * DamageMod);
                proj.Pierce = Pierce;
                proj.Transform.Pos = Transform.Pos;
                Vector2 mousePos = world.Camera.ScreenToWorldSpace(Input.Instance.MousePosition);
                Vector2 direction = (mousePos - Transform.Pos).Normalized();
                proj.Velocity = direction * 200.0f;
                proj.Transform.Rot = MathF.Atan2(direction.Y, direction.X) - MathF.PI / 2.0f;
            }

            // XP/HP point interactions
            foreach (Entity entity in world.SpatialQuery(Transform.Pos, 128.0f)) {
                if (entity is XpPoint) {
                    XpPoint xp = (XpPoint) entity;
                    Vector2 diff = xp.Transform.Pos - Transform.Pos;
                    float distance = diff.Magnitude();
                    diff.Normalize();
                    float speed = 10.0f * MathF.Exp(4.0f * (128.0f - distance) / 128.0f);
                    diff *= speed;
                    xp.Transform.Pos -= diff * deltaTime;
                }
            }
        }

        public override void OnCollision(Entity other) {
            if (other is XpPoint) {
                XpPoint xpEnt = (XpPoint) other;
                xpEnt.Kill();
                Xp++;

                if (Xp >= NeededXp) {
                    PickUpgrades();
                    LeveledWithoutUpgrade = true;
                    NeededXp = (int) (NeededXp * 1.25f);
                    Xp = 0;
                }
            }
        }

        private void PickUpgrades() {
            // https://stackoverflow.com/questions/5411694/get-all-inherited-classes-of-an-abstract-class
            Assembly? assembly = Assembly.GetAssembly(typeof(Upgrade));
            if (assembly == null) {
                throw new Exception("Failed to get assembly.");
            }
            List<Type> possibleUpgrades = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Upgrade))).ToList();

            Random rng = new Random();
            for (int i = 0; i < Upgrades.Length; i++) {
                int upgradeIndex = rng.Next(possibleUpgrades.Count);
                Type upgradeType = possibleUpgrades[upgradeIndex];
                possibleUpgrades.RemoveAt(upgradeIndex);
                Upgrade? upgrade = (Upgrade?) Activator.CreateInstance(upgradeType);
                if (upgrade == null) {
                    throw new Exception($"Couldn't create upgrade of type {upgradeType}.");
                }
                Upgrades[i] = upgrade;
            }
        }
    }
}
