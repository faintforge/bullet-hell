namespace BulletHell {
    public class FireRateUpgrade : Upgrade {
        public FireRateUpgrade() {
            Name = "Fast Pew Pew";
            Description = new string[] {
                "+18% fire rate",
            };
        }

        public override void Apply(Player player) {
            player.ShootDelay *= 1.0f - 0.18f;
        }
    }
}
