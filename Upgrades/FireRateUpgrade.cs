namespace BulletHell {
    public class FireRateUpgrade : Upgrade {
        public FireRateUpgrade() {
            Name = "Fast Pew Pew";
            Description = new string[] {
                "+8% fire rate",
            };
        }

        public override void Apply(Player player) {
            player.ShootDelay *= 0.92f;
        }
    }
}