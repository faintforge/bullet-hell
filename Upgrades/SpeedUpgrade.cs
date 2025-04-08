namespace BulletHell {
    public class SpeedUprgade : Upgrade {
        public SpeedUprgade() {
            Name = "Light Step";
            Description = new string[] {
                "+18% speed",
            };
        }

        public override void Apply(Player player) {
            player.Speed *= 1.18f;
        }
    }
}